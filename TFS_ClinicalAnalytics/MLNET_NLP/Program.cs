using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data.SqlClient;
using System.Text;

namespace MLNET_NLP
{
    class Program
    {



        static bool blTrain = true;


        static void Main(string[] args)
        {
            //https://stackoverflow.com/questions/8911345/text-mining-in-c-sharp
            //If you mean sentences/words/phrases/etc .. there is a service you can call .. opencalais.com, it attempts to identify entities within text. Also, you may want to look into the natural language toolkit .. nltk.org .. hope this helps .. also, you may find more of what you are looking for using "data mining"
            //information extraction (IE) .NET SUPPORTED??????? 
            //GOOGLE TEXT MINING
            //https://www.google.com/search?rlz=1C1GCEA_enUS862US862&sxsrf=ALeKk03GSQ0ZwlaphLcmXzRaiSeQYXeytw%3A1590071401469&ei=aZDGXrqOHIi7tAaPwoP4Aw&q=text+mining+c%23&oq=text+mining+c%23&gs_lcp=CgZwc3ktYWIQAzIGCAAQBxAeMgYIABAHEB4yBggAEAcQHjIGCAAQBxAeMgIIADoECAAQQzoICAAQCBAHEB5Qlk9YxWpghmxoAHAAeACAAbYBiAHaCJIBAzAuOJgBAKABAaoBB2d3cy13aXo&sclient=psy-ab&ved=0ahUKEwj65rTIlcXpAhWIHc0KHQ_hAD8Q4dUDCAw&uact=5
            //TMP GRAB COVID.COVID_ICUE_OCR_Training




            cleanResultsTest();
            return;












            //string[] strClassificationsArr = ("Hydroxychlo,Chloroquine,Zithromax,VitD,Magnesium,Prone,ICU,Steroid,Remdesivir,EIDD_2801,Antibiotic,Rocephin,Ceftriaxone,Other_Antibio,Zinc,Plasma,Hyperbar_O2,Avigan_Favip,Acterma_Tociliz,Kevzara_Sarilumb,Montelu_Singul,Vit_C,Lovenox_Hep,Aspirin_ASA,Vent,Intubated,Vent_Split,Fever_PosCovid,PE_Bloodclot,PulmEmb_DVT_Clot,Pulm_Embolism,DVT_Bloodclot,Atazanavir,Tenofovir,Lamivudine,Ritonavir").Split(',');


            string[] strClassificationsArr = ("Admit_ICU_Status,Prone_Position ,Ventilator ,Ventilator_Split ,Hydroxychloroquine ,Azithromycin ,Azithro_Hydroxychlor ,Azithro_Hydroxychl_Zinc ,Steroid_Use ,Remdesivir ,EIDD_2801 ,Ceftriax_Rocephin ,Other_Antibiotics ,Zinc_Suppl ,Plasma_Use ,Hyperbaric_O2 ,Avigan_Favipiravir ,Actemra_Tociliz ,Kevzara_Sarilumb ,Monteluk_Singulair ,Vit_C ,Vit_D ,Magnesium ,Anticoagulant ,Aspirin ,Atazanavir ,Tenofov_Lam_Riton").Split(',');

           // strClassificationsArr = ("Atazanavir ,Tenofov_Lam_Riton").Split(',');



            //ModelBuilder.strSQL = "SELECT  [pdf_id] ,[Hydroxychlo] ,[Chloroquine] ,[Zithromax] ,[VitD] ,[Magnesium] ,[Prone] ,[ICU] ,[Steroid] ,[Remdesivir] ,[EIDD_2801] ,[Antibiotic] ,[Rocephin] ,[Ceftriaxone] ,[Other_Antibio] ,[Zinc] ,[Plasma] ,[Hyperbar_O2] ,[Avigan_Favip] ,[Acterma_Tociliz] ,[Kevzara_Sarilumb] ,[Montelu_Singul] ,[Vit_C] ,[Lovenox_Hep] ,[Aspirin_ASA] ,[Vent] ,[Intubated] ,[Vent_Split] ,[Fever_PosCovid] ,[PE_Bloodclot] ,[PulmEmb_DVT_Clot] ,[Pulm_Embolism] ,[DVT_Bloodclot] ,[Atazanavir] ,[Tenofovir] ,[Lamivudine] ,[Ritonavir] ,[pdf_text] FROM [dbo].[covid19_pdf_members] WHERE approved = " + strDontTrain;
            ModelBuilder.strSQLConnectionString = @"Data Source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";

           // ModelBuilder.strSQL = "SELECT [pdf_id] ,[Hydroxychlo] ,[Chloroquine] ,[Zithromax] ,[VitD] ,[Magnesium] ,[Prone] ,[ICU] ,[Steroid] ,[Remdesivir] ,[EIDD_2801] ,[Antibiotic] ,[Rocephin] ,[Ceftriaxone] ,[Other_Antibio] ,[Zinc] ,[Plasma] ,[Hyperbar_O2] ,[Avigan_Favip] ,[Acterma_Tociliz] ,[Kevzara_Sarilumb] ,[Montelu_Singul] ,[Vit_C] ,[Lovenox_Hep] ,[Aspirin_ASA] ,[Vent] ,[Intubated] ,[Vent_Split] ,[Fever_PosCovid] ,[PE_Bloodclot] ,[PulmEmb_DVT_Clot] ,[Pulm_Embolism] ,[DVT_Bloodclot] ,[Atazanavir] ,[Tenofovir] ,[Lamivudine] ,[Ritonavir] ,[pdf_text] FROM [dbo].[covid19_pdf_members]"; //approved = 1 !!!!!

            ModelBuilder.strSQL = "SELECT LTRIM(RTRIM([SRN_ECAA])) as pdf_id, CAST([Admit_ICU_StatusN] as BIT) as Admit_ICU_Status, CAST([Prone_PositionN] as BIT) as Prone_Position, CAST([VentilatorN] as BIT) as Ventilator, CAST([Ventilator_SplitN] as BIT) as Ventilator_Split, CAST([HydroxychloroquineN] as BIT) as Hydroxychloroquine, CAST([AzithromycinN] as BIT) as Azithromycin, CAST([Azithro_HydroxychlorN] as BIT) as Azithro_Hydroxychlor, CAST([Azithro_Hydroxychl_ZincN] as BIT) as Azithro_Hydroxychl_Zinc, CAST([Steroid_UseN] as BIT) as Steroid_Use, CAST([RemdesivirN] as BIT) as Remdesivir, CAST([EIDD_2801N] as BIT) as EIDD_2801, CAST([Ceftriax_RocephinN] as BIT) as Ceftriax_Rocephin, CAST([Other_AntibioticsN] as BIT) as Other_Antibiotics, CAST([Zinc_SupplN] as BIT) as Zinc_Suppl, CAST([Plasma_UseN] as BIT) as Plasma_Use, CAST([Hyperbaric_O2N] as BIT) as Hyperbaric_O2, CAST([Avigan_FavipiravirN] as BIT) as Avigan_Favipiravir, CAST([Actemra_TocilizN] as BIT) as Actemra_Tociliz, CAST([Kevzara_SarilumbN] as BIT) as Kevzara_Sarilumb, CAST([Monteluk_SingulairN] as BIT) as Monteluk_Singulair, CAST([Vit_CN] as BIT) as Vit_C, CAST([Vit_DN] as BIT) as Vit_D, CAST([MagnesiumN] as BIT) as Magnesium, CAST([AnticoagulantN] as BIT) as Anticoagulant, CAST([AspirinN] as BIT) as Aspirin, CAST([AtazanavirN] as BIT) as Atazanavir, CAST([Tenofov_Lam_RitonN] as BIT) as Tenofov_Lam_Riton, pdf_text FROM [IL_UCA].[dbo].[covid19_pdf_mbr_train] WHERE isnull(pdf_text,'') <> ''";


        



            string strModelPath = null;
            ITransformer mlModel = null;
            
            foreach (string s in strClassificationsArr)
            {

                Console.WriteLine(s + "  STARTING...");
                // Load model & create prediction engine
                strModelPath = ModelBuilder.MODEL_FILEPATH.Replace("{$classification}", s.Trim());

                if (File.Exists(strModelPath.Trim()))
                    continue;


                if (!File.Exists(strModelPath) || blTrain) //CREATE MODELS
                {
                    if (File.Exists(strModelPath))
                    {
                        Console.WriteLine(s + "  EXISTS SKIPPING...");
                        continue;
                    }


                    try
                    {
                        ModelBuilder.CreateModel(s.Trim());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(s + " ERROR: " + e.Message);
                        continue;
                    }


                }
                else //CONSUME MODELS
                {

                    //ModelInput sampleData = ModelBuilder.CreateSingleDataSample();
                    IEnumerable<ModelInput> sampleDataArr = ModelBuilder.CreateMultipleDataSample();


                    mlModel = ModelBuilder.mlContext.Model.Load(strModelPath, out var modelInputSchema);
                    foreach (var sampleData in sampleDataArr)
                    {
                        var predictionResult = ModelBuilder.Predict(mlModel, sampleData);

                        Console.WriteLine("Using model to make single prediction -- Comparing actual Zithromax with predicted Zithromax from sample data...\n\n");
                        Console.WriteLine($"pdf_id: {sampleData.Pdf_id}");
                        Console.WriteLine($"Admit_ICU_Status: {sampleData.Admit_ICU_Status}");
                        Console.WriteLine($"Prone_Position: {sampleData.Prone_Position}");
                        Console.WriteLine($"Ventilator: {sampleData.Ventilator}");
                        Console.WriteLine($"Ventilator_Split: {sampleData.Ventilator_Split}");
                        Console.WriteLine($"Hydroxychloroquine: {sampleData.Hydroxychloroquine}");
                        Console.WriteLine($"Azithromycin: {sampleData.Azithromycin}");
                        Console.WriteLine($"Azithro_Hydroxychlor: {sampleData.Azithro_Hydroxychlor}");
                        Console.WriteLine($"Azithro_Hydroxychl_Zinc: {sampleData.Azithro_Hydroxychl_Zinc}");
                        Console.WriteLine($"Steroid_Use: {sampleData.Steroid_Use}");
                        Console.WriteLine($"Remdesivir: {sampleData.Remdesivir}");
                        Console.WriteLine($"EIDD_2801: {sampleData.EIDD_2801}");
                        Console.WriteLine($"Ceftriax_Rocephin: {sampleData.Ceftriax_Rocephin}");
                        Console.WriteLine($"Other_Antibiotics: {sampleData.Other_Antibiotics}");
                        Console.WriteLine($"Zinc_Suppl: {sampleData.Zinc_Suppl}");
                        Console.WriteLine($"Plasma_Use: {sampleData.Plasma_Use}");
                        Console.WriteLine($"Hyperbaric_O2 : {sampleData.Hyperbaric_O2 }");
                        Console.WriteLine($"Avigan_Favipiravir: {sampleData.Avigan_Favipiravir}");
                        Console.WriteLine($"Actemra_Tociliz: {sampleData.Actemra_Tociliz}");
                        Console.WriteLine($"Kevzara_Sarilumb: {sampleData.Kevzara_Sarilumb}");
                        Console.WriteLine($"Monteluk_Singulair: {sampleData.Monteluk_Singulair}");
                        Console.WriteLine($"Vit_C: {sampleData.Vit_C}");
                        Console.WriteLine($"Vit_D: {sampleData.Vit_D}");
                        Console.WriteLine($"Magnesium: {sampleData.Magnesium}");
                        Console.WriteLine($"Anticoagulant: {sampleData.Anticoagulant}");
                        Console.WriteLine($"Aspirin: {sampleData.Aspirin}");
                        Console.WriteLine($"Atazanavir: {sampleData.Atazanavir}");
                        Console.WriteLine($"Tenofov_Lam_Riton: {sampleData.Tenofov_Lam_Riton}");
                        //Console.WriteLine($"pdf_text: {sampleData.Pdf_text}");
                        //Console.WriteLine($"\n\nActual Zithromax: {sampleData.Zithromax} \nPredicted Zithromax: {predictionResult.Prediction}\n\n");
                        Console.WriteLine($"\n\nActual "+s+": "+ sampleData.GetType().GetProperty(s).GetValue(sampleData, null) + " \nPredicted " + s + ": " + predictionResult.Prediction +  "\n\n");


                        Console.WriteLine("=============== End of process, hit any key to finish ===============");
                        Console.ReadKey();

                        // Make a single prediction on the sample data and print results
                        //var predictionResult = ModelBuilder.Predict(sampleData, s);
                    }


                }

            }



            //if (blTrain)
            //{
            //    model = BuildAndTrainModel(_mlContext, splitDataView.TrainSet);
            //}
            //else
            //{
            //    DataViewSchema modelSchema;
            //    model = _mlContext.Model.Load(_modelPath, out modelSchema);
            //}




            //Evaluate(_mlContext, model, splitDataView.TestSet);

            //UseModelWithSingleItem(_mlContext, model);


            //UseModelWithBatchItems(_mlContext, model);



            //// Save model
            //if (blTrain)
            //    _mlContext.Model.Save(model, _trainingDataView.Schema, _modelPath);

            ////Extract Features and transform the data
            //var pipeline = ProcessData();

            ////Build and train the model
            //var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);

            ////Evaluate the model
            //Evaluate(_trainingDataView.Schema);

            ////Deploy and Predict with a model
            //PredictIssue();
        }




        static void cleanResultsTest()
        {
           

            string[] strClassificationsArr = ("Admit_ICU_Status,Prone_Position,Ventilator ,Ventilator_Split ,Hydroxychloroquine ,Azithromycin ,Azithro_Hydroxychlor ,Azithro_Hydroxychl_Zinc ,Steroid_Use ,Remdesivir ,EIDD_2801 ,Ceftriax_Rocephin ,Other_Antibiotics ,Zinc_Suppl ,Plasma_Use ,Hyperbaric_O2 ,Avigan_Favipiravir ,Actemra_Tociliz ,Kevzara_Sarilumb ,Monteluk_Singulair ,Vit_C ,Vit_D ,Magnesium ,Anticoagulant ,Aspirin ,Atazanavir ,Tenofov_Lam_Riton").Split(',');

            ModelBuilder.strSQLConnectionString = @"Data Source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";


            ModelBuilder.strSQL = "SELECT [pdf_id] ,[Admit_ICU_Status] ,[Prone_Position] ,[Ventilator] ,[Ventilator_Split] ,[Hydroxychloroquine] ,[Azithromycin] ,[Azithro_Hydroxychlor] ,[Azithro_Hydroxychl_Zinc] ,[Steroid_Use] ,[Remdesivir] ,[EIDD_2801] ,[Ceftriax_Rocephin] ,[Other_Antibiotics] ,[Zinc_Suppl] ,[Plasma_Use] ,[Hyperbaric_O2] ,[Avigan_Favipiravir] ,[Actemra_Tociliz] ,[Kevzara_Sarilumb] ,[Monteluk_Singulair] ,[Vit_C] ,[Vit_D] ,[Magnesium] ,[Anticoagulant] ,[Aspirin] ,[Atazanavir] ,[Tenofov_Lam_Riton] ,[pdf_text] FROM [dbo].[covid19_pdf_mbr] WHERE pdf_folder <> '\\\\nasv0048\\ucs_ca\\PHS_DATA_NEW\\Home Directory - COVID19\\ECAA_Documentation\\Random_Sample' ";


            string strModelPath = null;
            ITransformer mlModel = null;


            IEnumerable<ModelInput> sampleDataArr = ModelBuilder.CreateMultipleDataSample();

            StringBuilder sbSQL = new StringBuilder();

            int intCnt = 1;
            foreach (string s in strClassificationsArr)
            {
                intCnt = 1;
                Console.WriteLine(s + "  STARTING...");
                // Load model & create prediction engine
                strModelPath = ModelBuilder.MODEL_FILEPATH.Replace("{$classification}", s.Trim()).Trim();

                if (!File.Exists(strModelPath))
                    continue;

        
                //ModelInput sampleData = ModelBuilder.CreateSingleDataSample();



                mlModel = ModelBuilder.mlContext.Model.Load(strModelPath, out var modelInputSchema);
                foreach (var sampleData in sampleDataArr)
                {
                    var predictionResult = ModelBuilder.Predict(mlModel, sampleData);

                    Console.WriteLine(intCnt + " - Using model to make single prediction -- Comparing actual " + s.Trim() + " with predicted "+ s.Trim() + " from sample data...\n\n");
                    Console.WriteLine($"pdf_id: {sampleData.Pdf_id}");
                    Console.WriteLine($"Admit_ICU_Status: {sampleData.Admit_ICU_Status}");
                    Console.WriteLine($"Prone_Position: {sampleData.Prone_Position}");
                    Console.WriteLine($"Ventilator: {sampleData.Ventilator}");
                    Console.WriteLine($"Ventilator_Split: {sampleData.Ventilator_Split}");
                    Console.WriteLine($"Hydroxychloroquine: {sampleData.Hydroxychloroquine}");
                    Console.WriteLine($"Azithromycin: {sampleData.Azithromycin}");
                    Console.WriteLine($"Azithro_Hydroxychlor: {sampleData.Azithro_Hydroxychlor}");
                    Console.WriteLine($"Azithro_Hydroxychl_Zinc: {sampleData.Azithro_Hydroxychl_Zinc}");
                    Console.WriteLine($"Steroid_Use: {sampleData.Steroid_Use}");
                    Console.WriteLine($"Remdesivir: {sampleData.Remdesivir}");
                    Console.WriteLine($"EIDD_2801: {sampleData.EIDD_2801}");
                    Console.WriteLine($"Ceftriax_Rocephin: {sampleData.Ceftriax_Rocephin}");
                    Console.WriteLine($"Other_Antibiotics: {sampleData.Other_Antibiotics}");
                    Console.WriteLine($"Zinc_Suppl: {sampleData.Zinc_Suppl}");
                    Console.WriteLine($"Plasma_Use: {sampleData.Plasma_Use}");
                    Console.WriteLine($"Hyperbaric_O2 : {sampleData.Hyperbaric_O2 }");
                    Console.WriteLine($"Avigan_Favipiravir: {sampleData.Avigan_Favipiravir}");
                    Console.WriteLine($"Actemra_Tociliz: {sampleData.Actemra_Tociliz}");
                    Console.WriteLine($"Kevzara_Sarilumb: {sampleData.Kevzara_Sarilumb}");
                    Console.WriteLine($"Monteluk_Singulair: {sampleData.Monteluk_Singulair}");
                    Console.WriteLine($"Vit_C: {sampleData.Vit_C}");
                    Console.WriteLine($"Vit_D: {sampleData.Vit_D}");
                    Console.WriteLine($"Magnesium: {sampleData.Magnesium}");
                    Console.WriteLine($"Anticoagulant: {sampleData.Anticoagulant}");
                    Console.WriteLine($"Aspirin: {sampleData.Aspirin}");
                    Console.WriteLine($"Atazanavir: {sampleData.Atazanavir}");
                    Console.WriteLine($"Tenofov_Lam_Riton: {sampleData.Tenofov_Lam_Riton}");
                    //Console.WriteLine($"pdf_text: {sampleData.Pdf_text}");
                    //Console.WriteLine($"\n\nActual Zithromax: {sampleData.Zithromax} \nPredicted Zithromax: {predictionResult.Prediction}\n\n");
                    Console.WriteLine($"\n\nActual " + s + ": " + sampleData.GetType().GetProperty(s.Trim()).GetValue(sampleData, null) + " \nPredicted " + s + ": " + predictionResult.Prediction + "\n\n");

                    //IF TRUE?
                    if(predictionResult.Prediction)
                    {
                        //Console.WriteLine("=============== End of process, hit any key to finish ===============");
                        //Console.ReadKey();
                    }


                    sbSQL.AppendLine("UPDATE [dbo].[covid19_pdf_mbr] SET " + s.Trim() + " =  " + (predictionResult.Prediction ? 1 : 0) + " WHERE pdf_id = '" + sampleData.Pdf_id + "';");


                    intCnt++;


                    // Make a single prediction on the sample data and print results
                    //var predictionResult = ModelBuilder.Predict(sampleData, s);

                }

                DBConnection64.ExecuteMSSQL(ModelBuilder.strSQLConnectionString, sbSQL.ToString());
                sbSQL.Remove(0, sbSQL.Length);

            }



            //if (blTrain)
            //{
            //    model = BuildAndTrainModel(_mlContext, splitDataView.TrainSet);
            //}
            //else
            //{
            //    DataViewSchema modelSchema;
            //    model = _mlContext.Model.Load(_modelPath, out modelSchema);
            //}




            //Evaluate(_mlContext, model, splitDataView.TestSet);

            //UseModelWithSingleItem(_mlContext, model);


            //UseModelWithBatchItems(_mlContext, model);



            //// Save model
            //if (blTrain)
            //    _mlContext.Model.Save(model, _trainingDataView.Schema, _modelPath);

            ////Extract Features and transform the data
            //var pipeline = ProcessData();

            ////Build and train the model
            //var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);

            ////Evaluate the model
            //Evaluate(_trainingDataView.Schema);

            ////Deploy and Predict with a model
            //PredictIssue();
        }






















        public static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations 
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("pdf_text_tf", "pdf_text")
                                      .Append(mlContext.Transforms.CopyColumns("Features", "pdf_text_tf"));
            // Set the training algorithm 
            var trainer = mlContext.BinaryClassification.Trainers.LightGbm(labelColumnName: "Zithromax", featureColumnName: "Features");

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




        private static void Evaluate(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            // Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            // in order to evaluate and get the model's accuracy metrics
            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.BinaryClassification.CrossValidateNonCalibrated(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Zithromax");
            PrintBinaryClassificationFoldsAverageMetrics(crossValidationResults);
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






        //public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        //{
        //    var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(ModelInput.pdf_text))
        //        .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Zithromax", featureColumnName: "Features"));


        //    Console.WriteLine("=============== Create and Train the Model ===============");
        //    var model = estimator.Fit(splitTrainSet);
        //    Console.WriteLine("=============== End of training ===============");
        //    Console.WriteLine();


        //    return model;
        //}

        //public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        //{
        //    Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
        //    IDataView predictions = model.Transform(splitTestSet);

        //    CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Zithromax");

        //    Console.WriteLine();
        //    Console.WriteLine("Model quality metrics evaluation");
        //    Console.WriteLine("--------------------------------");
        //    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
        //    Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
        //    Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        //    Console.WriteLine("=============== End of model evaluation ===============");


        //}

        //private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        //{
        //    PredictionEngine<ModelInput, ModelOutput> predictionFunction = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);


        //    ModelInput sampleStatement = new ModelInput
        //    {
        //       Pdf_text = "This was a very bad steak Zithromax"
        //    };

        //    var resultPrediction = predictionFunction.Predict(sampleStatement);


        //    Console.WriteLine();
        //    Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

        //    Console.WriteLine();
        //    Console.WriteLine($"Sentiment: {resultPrediction.pdf_text} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

        //    Console.WriteLine("=============== End of Predictions ===============");
        //    Console.WriteLine();


        //}


        //public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        //{
        //    IEnumerable<ModelInput> sentiments = new[]
        //        {
        //            new ModelInput
        //            {
        //               Pdf_text = "This was a horrible meal Zithromax"
        //            },
        //            new ModelInput
        //            {
        //                Pdf_text = "I love this spaghetti."
        //            }
        //        };


        //    IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

        //    IDataView predictions = model.Transform(batchComments);

        //    // Use model to predict whether comment data is Positive (1) or Negative (0).
        //    IEnumerable<ModelOutput> predictedResults = mlContext.Data.CreateEnumerable<ModelOutput>(predictions, reuseRowObject: false);


        //    Console.WriteLine();

        //    Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");


        //    foreach (ModelOutput prediction in predictedResults)
        //    {
        //        Console.WriteLine($"Sentiment: {prediction.pdf_text} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
        //    }
        //    Console.WriteLine("=============== End of predictions ===============");


        //}




        //        public static IEstimator<ITransformer> ProcessData()
        //        {
        //            //1 use the MapValueToKey() method to transform the Area column into a numeric key type Label column
        //            //2 transforms the text (Title and Description) columns into a numeric vector for each called TitleFeaturized and DescriptionFeaturized
        //            //3 combines all of the feature columns into the Features column using the Concatenate() method
        //            //4 append a AppendCacheCheckpoint to cache the DataView so when you iterate over the data multiple times using the cache might get better performance
        //            //var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Area", outputColumnName: "Label")
        //            //    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Title", outputColumnName: "TitleFeaturized"))
        //            //    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Description", outputColumnName: "DescriptionFeaturized"))
        //            //    .Append(_mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized")).AppendCacheCheckpoint(_mlContext);



        //            //1 use the MapValueToKey() method to transform the Area column into a numeric key type Label column
        //            //2 transforms the text (Title and Description) columns into a numeric vector for each called TitleFeaturized and DescriptionFeaturized
        //            //3 combines all of the feature columns into the Features column using the Concatenate() method
        //            //4 append a AppendCacheCheckpoint to cache the DataView so when you iterate over the data multiple times using the cache might get better performance
        //            var pipelineHydroxychlo = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Hydroxychlo", outputColumnName: "Label")
        //                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "pdf_text", outputColumnName: "TextFeaturized")).AppendCacheCheckpoint(_mlContext);

        //            var pipelineChloroquine = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Chloroquine", outputColumnName: "Label")
        //.Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "pdf_text", outputColumnName: "TextFeaturized")).AppendCacheCheckpoint(_mlContext);

        //            var pipelineZithromax = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Zithromax", outputColumnName: "Label")
        //    .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "pdf_text", outputColumnName: "TextFeaturized")).AppendCacheCheckpoint(_mlContext);


        //            return pipelineHydroxychlo.Append(pipelineChloroquine).Append(pipelineZithromax);

        //            //return pipeline;
        //        }

        //        public static IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        //        {

        //            //The SdcaMaximumEntropy is your multiclass classification training algorithm. 
        //            //This is appended to the pipeline and accepts the featurized Title and Description (Features) and the Label input parameters to learn from the historic data.
        //            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
        //        .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        //            //Fit the model to the splitTrainSet data and return the trained model 
        //            //The Fit()method trains your model by transforming the dataset and applying the training
        //            _trainedModel = trainingPipeline.Fit(trainingDataView);

        //            //The PredictionEngine is a convenience API, which allows you to pass in and then perform a prediction on a single instance of data
        //            _predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_trainedModel);

        //            //Add a ModelInput to test the trained model's prediction in the Predict method by creating an instance of ModelInput:
        //            ModelInput_Test data_to_process = new ModelInput_Test()
        //            {
        //                Title = "WebSockets communication is slow in my machine",
        //                Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
        //            };

        //            //Use the Predict() function makes a prediction on a single row of data
        //            var prediction = _predEngine.Predict(data_to_process);

        //            //Area label prediction in order to share the results and act on them accordingly
        //            Console.WriteLine($"=============== Single Prediction just-trained-model - Result: {prediction.Area} ===============");

        //            return trainingPipeline;
        //        }


        //        public static void Evaluate(DataViewSchema trainingDataViewSchema)
        //        {
        //            //load the test dataset by adding the following code to the Evaluate method
        //            var testDataView = _mlContext.Data.LoadFromTextFile<ModelInput_Test>(_testDataPath, hasHeader: true);

        //            //The Evaluate() method computes the quality metrics for the model using the specified dataset. 
        //            //It returns a MulticlassClassificationMetrics object that contains the overall metrics computed by multiclass classification evaluators
        //            // To display the metrics to determine the quality of the model, you need to get them first. 
        //            // Notice the use of the Transform() method of the machine learning _trainedModel global variable 
        //            var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));


        //            //following code to display the metrics, share the results, and then act on them
        //            Console.WriteLine($"*************************************************************************************************************");
        //            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
        //            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
        //            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
        //            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
        //            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
        //            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
        //            Console.WriteLine($"*************************************************************************************************************");


        //            //Once satisfied with your model, save it to a file to make predictions at a later time or in another application
        //            SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);

        //        }

        //        private static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        //        {

        //            //This code uses the Save method to serialize and store the trained model as a zip file
        //            mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);
        //        }


        //        private static void PredictIssue()
        //        {
        //            //Load the saved model into your application by adding the following code to the PredictIssue method:
        //            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

        //            //Add a ModelInput to test the trained model's prediction in the Predict method by creating an instance of ModelInput:
        //            ModelInput_Test singleIssue = new ModelInput_Test() { Title = "Entity Framework crashes", Description = "When connecting to the database, EF is crashing" };

        //            //The PredictionEngine is a convenience API, which allows you to perform a prediction on a single instance of data.
        //            //use the PredictionEnginePool service, which creates an ObjectPool of PredictionEngine objects for use throughout your application
        //            _predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput_Test, ModelOutput_Test>(loadedModel);

        //            //Use the PredictionEngine to predict the Area GitHub label by adding the following code to the PredictIssue method for the prediction:
        //            var prediction = _predEngine.Predict(singleIssue);

        //            //Create a display for the results 
        //            Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============");
        //        }


    }

}
