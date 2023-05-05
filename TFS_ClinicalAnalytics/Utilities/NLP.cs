using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class NLP
    {



        public static char[] charSeparatorsArr = null;
        public static string[] strStopwordsArr = {"i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself", "yourselves", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their", "theirs", "themselves", "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", "because", "as", "until", "while", "of", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before", "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don", "should", "now"};
        public static bool blUseStopWords = true;


        static readonly char[] charSeparatorsArrGLOBAL = { ' ', '.', ',' ,'\n', '\t', '_', ':', '#' };


        public static void FeaturizeText()
        {
            // Create a new ML context, for ML.NET operations. It can be used for
            // exception tracking and logging, as well as the source of randomness.
            var mlContext = new MLContext();

            // Create a small dataset as an IEnumerable.
            var samples = new List<TextData>()
            {
                new TextData(){ Text = "ML.NET's FeaturizeText API uses a " +
                    "composition of several basic transforms to convert text " +
                    "into numeric features." },

                new TextData(){ Text = "This API can be used as a featurizer to " +
                    "perform text classification." },

                new TextData(){ Text = "There are a number of approaches to text " +
                    "classification." },

                new TextData(){ Text = "One of the simplest and most common " +
                    "approaches is called “Bag of Words”." },

                new TextData(){ Text = "Text classification can be used for a " +
                    "wide variety of tasks" },

                new TextData(){ Text = "such as sentiment analysis, topic " +
                    "detection, intent identification etc." },
            };

            // Convert training data to IDataView.
            var dataview = mlContext.Data.LoadFromEnumerable(samples);

            // A pipeline for converting text into numeric features.
            // The following call to 'FeaturizeText' instantiates 
            // 'TextFeaturizingEstimator' with default parameters.
            // The default settings for the TextFeaturizingEstimator are
            //      * StopWordsRemover: None
            //      * CaseMode: Lowercase
            //      * OutputTokensColumnName: None
            //      * KeepDiacritics: false, KeepPunctuations: true, KeepNumbers:
            //          true
            //      * WordFeatureExtractor: NgramLength = 1
            //      * CharFeatureExtractor: NgramLength = 3, UseAllLengths = false
            // The length of the output feature vector depends on these settings.
            var textPipeline = mlContext.Transforms.Text.FeaturizeText("Features",
                "Text");

            // Fit to data.
            var textTransformer = textPipeline.Fit(dataview);

            // Create the prediction engine to get the features extracted from the
            // text.
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TextData,
                TransformedTextData>(textTransformer);

            // Convert the text into numeric features.
            var prediction = predictionEngine.Predict(samples[0]);

            // Print the length of the feature vector.
            Console.WriteLine($"Number of Features: {prediction.Features.Length}");

            // Print the first 10 feature values.
            Console.Write("Features: ");
            for (int i = 0; i < 10; i++)
                Console.Write($"{prediction.Features[i]:F4}  ");

            //  Expected output:
            //   Number of Features: 332
            //   Features: 0.0857  0.0857  0.0857  0.0857  0.0857  0.0857  0.0857  0.0857  0.0857  0.1715 ...
        }

        public static void TextToStringTokens(string strText)
        {
            if (charSeparatorsArr == null)
                charSeparatorsArr = charSeparatorsArrGLOBAL;



            //UNCOMMENT ME!!!! ???
           // strStopwordsArr = null;


            var mlContext = new MLContext();
            var emptyData = new List<TextData>();

            var data = mlContext.Data.LoadFromEnumerable(emptyData);

            TextTokens textTokens = null;

            //1 OR 2  OR 3
            if (blUseStopWords)
            {
                if(strStopwordsArr == null)
                {

                    var tokenization = mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text", separators: charSeparatorsArr).Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Tokens", "Tokens", Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English));

                    var tokenModel = tokenization.Fit(data);
                    var engine = mlContext.Model.CreatePredictionEngine<TextData, TextTokens>(tokenModel);

                    textTokens = engine.Predict(new TextData { Text = strText });

                }
                else
                {

                    var tokenization = mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text", separators: charSeparatorsArr).Append(mlContext.Transforms.Text.RemoveStopWords("Tokens", "Tokens", stopwords: strStopwordsArr));

                    var tokenModel = tokenization.Fit(data);
                    var engine = mlContext.Model.CreatePredictionEngine<TextData, TextTokens>(tokenModel);

                    textTokens = engine.Predict(new TextData { Text = strText });


                }
            }
            else
            {
                var tokenization = mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text", separators: charSeparatorsArr);

                var tokenModel = tokenization.Fit(data);
                var engine = mlContext.Model.CreatePredictionEngine<TextData, TextTokens>(tokenModel);

                textTokens = engine.Predict(new TextData { Text = strText });
            }




            KeyWordData[] keywords = new KeyWordData[]
            {
                new KeyWordData
                {
                    Category = "Vent",
                    Keyword ="Vent",
                    Rating=4.7f
                },
                new KeyWordData
                {
                    Category = "Vent",
                    Keyword ="vc",
                    Rating=4.7f
                },
                new KeyWordData
                {
                    Category = "Magnesium",
                    Keyword ="mg",
                    Rating=4.7f
                }
            };


            var dataview = mlContext.Data.LoadFromEnumerable(keywords);



            // Define text transform estimator
            var textEstimator = mlContext.Transforms.Text.FeaturizeText("Keyword");
            // Fit data to estimator
            // Fitting generates a transformer that applies the operations of defined by estimator
            ITransformer textTransformer = textEstimator.Fit(data);
            // Transform data
            IDataView transformedData = textTransformer.Transform(data);




        }



        public static void TextToCharTokens(string strText)
        {
            var context = new MLContext();
            var emptyData = new List<TextData>();

            var data = context.Data.LoadFromEnumerable(emptyData);

            var charTokenization = context.Transforms.Text.TokenizeIntoCharactersAsKeys("Tokens", "Text", useMarkerCharacters: false).Append(context.Transforms.Conversion.MapKeyToValue("Tokens"));

            var charTokenModel = charTokenization.Fit(data);

            var charEngine = context.Model.CreatePredictionEngine<TextData, TextTokens>(charTokenModel);
            var charTokens = charEngine.Predict(new TextData { Text = strText });

            //PrintTokens(charTokens);
            //Console.ReadLine();

        }




        private  static void PrintTokens(TextTokens tokens)
        {
            Console.WriteLine(Environment.NewLine);
            var sb = new StringBuilder();


            foreach(var token in tokens.Tokens)
            {
                sb.Append(token);
            }

            Console.Write(sb.ToString());

        }




    }


    internal class TextData      
    {
        public string Text { get; set; }
    }


    internal class TextTokens
    {
        public string[] Tokens { get; set; }
    }


    internal class TransformedTextData : TextData
    {
        public float[] Features { get; set; }
    }



    internal class KeyWordData
    {
        public string Category { get; set; }
        public string Keyword { get; set; }
        public float Rating { get; set; }
    }



}
