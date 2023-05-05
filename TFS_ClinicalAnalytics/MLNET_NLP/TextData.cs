using Microsoft.ML.Data;

namespace MLNET_NLP
{


    public class ModelInput
    {
        [ColumnName("pdf_id"), LoadColumn(0)]
        public string Pdf_id { get; set; }




        [ColumnName("Admit_ICU_Status"), LoadColumn(1)]
        public bool Admit_ICU_Status { get; set; }


        [ColumnName("Prone_Position"), LoadColumn(2)]
        public bool Prone_Position { get; set; }


        [ColumnName("Ventilator"), LoadColumn(3)]
        public bool Ventilator { get; set; }


        [ColumnName("Ventilator_Split"), LoadColumn(4)]
        public bool Ventilator_Split { get; set; }


        [ColumnName("Hydroxychloroquine"), LoadColumn(5)]
        public bool Hydroxychloroquine { get; set; }


        [ColumnName("Azithromycin"), LoadColumn(6)]
        public bool Azithromycin { get; set; }


        [ColumnName("Azithro_Hydroxychlor"), LoadColumn(7)]
        public bool Azithro_Hydroxychlor { get; set; }


        [ColumnName("Azithro_Hydroxychl_Zinc"), LoadColumn(8)]
        public bool Azithro_Hydroxychl_Zinc { get; set; }


        [ColumnName("Steroid_Use"), LoadColumn(9)]
        public bool Steroid_Use { get; set; }


        [ColumnName("Remdesivir"), LoadColumn(10)]
        public bool Remdesivir { get; set; }


        [ColumnName("EIDD_2801"), LoadColumn(11)]
        public bool EIDD_2801 { get; set; }


        [ColumnName("Ceftriax_Rocephin"), LoadColumn(12)]
        public bool Ceftriax_Rocephin { get; set; }


        [ColumnName("Other_Antibiotics"), LoadColumn(13)]
        public bool Other_Antibiotics { get; set; }


        [ColumnName("Zinc_Suppl"), LoadColumn(14)]
        public bool Zinc_Suppl { get; set; }


        [ColumnName("Plasma_Use"), LoadColumn(15)]
        public bool Plasma_Use { get; set; }


        [ColumnName("Hyperbaric_O2"), LoadColumn(16)]
        public bool Hyperbaric_O2 { get; set; }


        [ColumnName("Avigan_Favipiravir"), LoadColumn(17)]
        public bool Avigan_Favipiravir { get; set; }


        [ColumnName("Actemra_Tociliz"), LoadColumn(18)]
        public bool Actemra_Tociliz { get; set; }


        [ColumnName("Kevzara_Sarilumb"), LoadColumn(19)]
        public bool Kevzara_Sarilumb { get; set; }


        [ColumnName("Monteluk_Singulair"), LoadColumn(20)]
        public bool Monteluk_Singulair { get; set; }


        [ColumnName("Vit_C"), LoadColumn(21)]
        public bool Vit_C { get; set; }


        [ColumnName("Vit_D"), LoadColumn(22)]
        public bool Vit_D { get; set; }


        [ColumnName("Magnesium"), LoadColumn(23)]
        public bool Magnesium { get; set; }


        [ColumnName("Anticoagulant"), LoadColumn(24)]
        public bool Anticoagulant { get; set; }


        [ColumnName("Aspirin"), LoadColumn(25)]
        public bool Aspirin { get; set; }


        [ColumnName("Atazanavir"), LoadColumn(26)]
        public bool Atazanavir { get; set; }


        [ColumnName("Tenofov_Lam_Riton"), LoadColumn(27)]
        public bool Tenofov_Lam_Riton { get; set; }


        [ColumnName("pdf_text"), LoadColumn(28)]
        public string Pdf_text { get; set; }

    }


    public class ModelOutput
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Score { get; set; }
    }





    public class DataToProcess
    {
        [LoadColumn(0)]
        public string pdf_id { get; set; }
        [LoadColumn(1)]
        public bool Hydroxychlo { get; set; }
        [LoadColumn(2)]
        public bool Chloroquine { get; set; }
        [LoadColumn(3)]
        public bool Zithromax { get; set; }
        [LoadColumn(4)]
        public bool VitD { get; set; }
        [LoadColumn(5)]
        public string pdf_text { get; set; }

    }

    public class DataPrediction : DataToProcess
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }



    public class DataToProcess_Test
    {
        [LoadColumn(0)]
        public string ID { get; set; }
        [LoadColumn(1)]
        public string Area { get; set; }
        [LoadColumn(2)]
        public string Title { get; set; }
        [LoadColumn(3)]
        public string Description { get; set; }
    }

    public class DataPrediction_Test
    {
        [ColumnName("PredictedLabel")]
        public string Area;
    }
}
