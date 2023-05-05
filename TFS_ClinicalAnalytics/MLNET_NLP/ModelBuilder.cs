using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data.SqlClient;
using Microsoft.ML.Trainers;

namespace MLNET_NLP
{
    class ModelBuilder
    {

        //private static string TRAIN_DATA_FILEPATH = @"C:\Users\cgiorda\AppData\Local\Temp\b6abddf9-ca53-4db8-a4f9-5bee4e7a42f1.tsv";
       //public static string MODEL_FILEPATH = @"C:\Work\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\MLNET_NLP\Models\model_{$classification}.zip";
        public static string MODEL_FILEPATH = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - COVID19\~Automation_DONTDELETE\MLNET_Models\model_{$classification}.zip";
        // Create MLContext to be shared across the model creation workflow objects 
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        public static MLContext mlContext = new MLContext(seed: 1);


        private static IDataView _TrainTestDataView;


        public static string strSQL = null;
        public static string strSQLConnectionString = null;


        public static void CreateModel(string strClassification)
        {
            string strModelPath = MODEL_FILEPATH.Replace("{$classification}", strClassification);
            DataOperationsCatalog.TrainTestData dataSplit;
            // Load Data
            //trainingDataView = mlContext.Data.LoadFromTextFile<ModelInput>(
            //                                path: TRAIN_DATA_FILEPATH,
            //                                hasHeader: true,
            //                                separatorChar: '\t',
            //                                allowQuoting: true,
            //                                allowSparse: false);
            if (_TrainTestDataView == null)
            {
                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();
                DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, strSQLConnectionString, strSQL);
                _TrainTestDataView = loader.Load(dbSource);
            }
            //dataSplit = mlContext.Data.TrainTestSplit(_TrainTestDataView, testFraction: 0.2);
            //dataSplit = mlContext.Data.TrainTestSplit(_TrainTestDataView, testFraction: .99);


            // Build training pipeline
            IEstimator<ITransformer> trainingPipeline = BuildTrainingPipeline(mlContext, strClassification);

            // Evaluate quality of Model
            //Evaluate(mlContext, dataSplit.TrainSet, trainingPipeline, strClassification);
            Evaluate(mlContext, _TrainTestDataView, trainingPipeline, strClassification);

            ITransformer mlModel;
            if (File.Exists(strModelPath))
            {
                DataViewSchema modelSchema;
                mlModel = mlContext.Model.Load(strModelPath, out modelSchema);
            }
            else
            {
                // Train Model
               // mlModel = TrainModel(mlContext, dataSplit.TrainSet, trainingPipeline);
                mlModel = TrainModel(mlContext, _TrainTestDataView, trainingPipeline);
            }


            //DELETE AFTER TESTING BELOW IS IMPLEMENTED
            //DELETE AFTER TESTING BELOW IS IMPLEMENTED
            //DELETE AFTER TESTING BELOW IS IMPLEMENTED
            if (!File.Exists(strModelPath))
            {
                // Save model
                SaveModel(mlContext, mlModel, strModelPath, _TrainTestDataView.Schema);
            }
            else
            {
                //ReTrainModel(mlContext, dataSplit.TrainSet, mlModel);
                ReTrainModel(mlContext, _TrainTestDataView, mlModel);
            }

            //CHECK BEFORE SAVING!!!!!
            //CHECK BEFORE SAVING!!!!!
            //CHECK BEFORE SAVING!!!!!
            //CHECK BEFORE SAVING!!!!!
            //CHECK BEFORE SAVING!!!!!
            //CHECK BEFORE SAVING!!!!!
            //CHECK BEFORE SAVING!!!!!
            //var predictions = mlModel.Transform(dataSplit.TestSet);
            //var metrics = mlContext.Regression.Evaluate(predictions);
            //Console.WriteLine($"Average minimum score: {metrics.MeanAbsoluteError}");
            ////var predictFunction = mlModel.creaate
            ////LOOKS GOOD, SAVE OR RETRAIN
            //if(metrics.MeanAbsoluteError.ToString() != "NEVERTHIS")
            //{
            //    if (File.Exists(strModelPath))
            //    {
            //        // Save model
            //        SaveModel(mlContext, mlModel, strModelPath, _TrainTestDataView.Schema);
            //    }
            //    else
            //    {
            //        ReTrainModel(mlContext, dataSplit.TrainSet, mlModel);
            //    }
            //}


        }

        public static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext, string strClassification)
        {
            // Data process configuration with pipeline data transformations 
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("pdf_text_tf", "pdf_text")
                                      .Append(mlContext.Transforms.CopyColumns("Features", "pdf_text_tf"));
            // Set the training algorithm 
            var trainer = mlContext.BinaryClassification.Trainers.LightGbm(labelColumnName: strClassification, featureColumnName: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            return trainingPipeline;
        }

        public static ITransformer TrainModel(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            Console.WriteLine("=============== Training  model ===============");

            ITransformer model = trainingPipeline.Fit(trainingDataView);

            Console.WriteLine("=============== End of training process ===============");
            return model;
        }




        public static ITransformer ReTrainModel(MLContext mlContext, IDataView newDataView, ITransformer modelRetrain)
        {
            Console.WriteLine("=============== Re-Training  model ===============");

            LinearRegressionModelParameters originalModelParameters = ((ISingleFeaturePredictionTransformer<object>)modelRetrain).Model as LinearRegressionModelParameters;

            // Retrain model
            RegressionPredictionTransformer<LinearRegressionModelParameters> retrainedModel = mlContext.Regression.Trainers.OnlineGradientDescent().Fit(newDataView, originalModelParameters);

            ////Load New Data
            //IDataView newData = mlContext.Data.LoadFromEnumerable<HousingData>(housingData);

            //// Preprocess Data
            //IDataView transformedNewData = dataPrepPipeline.Transform(newData);


            Console.WriteLine("=============== End of Re-training process ===============");
            return retrainedModel;
        }




        private static void Evaluate(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline, string strClassification)
        {
            // Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            // in order to evaluate and get the model's accuracy metrics
            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.BinaryClassification.CrossValidateNonCalibrated(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: strClassification);
            PrintBinaryClassificationFoldsAverageMetrics(crossValidationResults);
        }



        private static void EvaluateModelQuality(MLContext mlContext, ITransformer testingTransformer, IDataView testingDataView)
        {

            //// Measure trained model performance
            //// Apply data prep transformer to test data
            //IDataView transformedTestData = testingTransformer.Transform(testingDataView);



            //// Use trained model to make inferences on test data
            //IDataView testDataPredictions = trainedModel.Transform(transformedTestData);

            //// Extract model metrics and get RSquared
            //RegressionMetrics trainedModelMetrics = mlContext.Regression.Evaluate(testDataPredictions);
            //double rSquared = trainedModelMetrics.RSquared;
        }




        private static void SaveModel(MLContext mlContext, ITransformer mlModel, string modelRelativePath, DataViewSchema modelInputSchema)
        {
            // Save/persist the trained model to a .ZIP file
            Console.WriteLine($"=============== Saving the model  ===============");
            mlContext.Model.Save(mlModel, modelInputSchema, GetAbsolutePath(modelRelativePath));
            Console.WriteLine("The model is saved to {0}", GetAbsolutePath(modelRelativePath));
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public static void PrintBinaryClassificationMetrics(BinaryClassificationMetrics metrics)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*       Metrics for binary classification model      ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"*       Auc:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"************************************************************");
        }


        public static void PrintBinaryClassificationFoldsAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<BinaryClassificationMetrics>> crossValResults)
        {
            var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

            var AccuracyValues = metricsInMultipleFolds.Select(m => m.Accuracy);
            var AccuracyAverage = AccuracyValues.Average();
            var AccuraciesStdDeviation = CalculateStandardDeviation(AccuracyValues);
            var AccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(AccuracyValues);


            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Binary Classification model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average Accuracy:    {AccuracyAverage:0.###}  - Standard deviation: ({AccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({AccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*************************************************************************************************************");
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
            return standardDeviation;
        }

        public static double CalculateConfidenceInterval95(IEnumerable<double> values)
        {
            double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
            return confidenceInterval95;
        }

        //CONSUME MODEL
        public static ModelOutput Predict(ModelInput input, string strClassification)
        {
            
            // Create new MLContext
            MLContext mlContext = new MLContext();

            // Load model & create prediction engine
            string strModelPath = MODEL_FILEPATH.Replace("{$classification}", strClassification);
            ITransformer mlModel = mlContext.Model.Load(strModelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Use model to make prediction on input data
            ModelOutput result = predEngine.Predict(input);
            return result;
        }

        public static ModelOutput Predict(ITransformer mlModel, ModelInput input)
        {

            // Create new MLContext
            MLContext mlContext = new MLContext();


            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Use model to make prediction on input data
            ModelOutput result = predEngine.Predict(input);
            return result;
        }
        //public static ModelOutput PredictMultiple(ModelInput input, string strClassification)
        //{

        //    // Create new MLContext
        //    MLContext mlContext = new MLContext();

        //    // Load model & create prediction engine
        //    string strModelPath = MODEL_FILEPATH.Replace("{$classification}", strClassification);
        //    ITransformer mlModel = mlContext.Model.Load(strModelPath, out var modelInputSchema);
        //    var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

        //    // Use model to make prediction on input data
        //    ModelOutput result = predEngine.Predict(input);
        //    return result;
        //}




        public static ModelInput CreateSingleDataSample()
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load dataset
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, strSQLConnectionString, strSQL);
            IDataView dataView = loader.Load(dbSource);

            // Use first line of dataset as model input
            // You can replace this with new test data (hardcoded or from end-user application)
            ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                                                        .First();
            return sampleForPrediction;
        }

        public static IEnumerable<ModelInput> CreateMultipleDataSample()
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load dataset
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, strSQLConnectionString, strSQL);
            IDataView dataView = loader.Load(dbSource);

            // Use first line of dataset as model input
            // You can replace this with new test data (hardcoded or from end-user application)
            IEnumerable<ModelInput> sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false);

            return sampleForPrediction;
        }

    }
}
