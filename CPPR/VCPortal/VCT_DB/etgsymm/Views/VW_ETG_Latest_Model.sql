CREATE VIEW [etgsymm].[VW_ETG_Latest_Model]
	AS /****** Script for SelectTopNRows command from SSMS  ******/
SELECT  c.[Premium_Specialty]
      ,c.[ETG_Base_Class]


	  ,p.[PC_Episode_Count] as PC_Episode_Count_Previous
      ,c.[PC_Episode_Count] as EC_Episode_Count_Currrent

	  ,p.[PC_Total_Cost] as PC_Total_Cost_Previous
      ,c.[PC_Total_Cost] as PC_Total_Cost_Currrent

	  	  ,p.[PC_Average_Cost] as PC_Average_Cost_Previous
      ,c.[PC_Average_Cost] as PC_Average_Cost_Currrent


	  	  ,p.[PC_Coefficients_of_Variation] as PC_Coefficients_of_Variation_Previous
      ,c.[PC_Coefficients_of_Variation] as PC_Coefficients_of_Variation_Currrent


	  	  ,p.[PC_Normalized_Pricing_Episode_Count] as PC_Normalized_Pricing_Episode_Count_Previous
      ,c.[PC_Normalized_Pricing_Episode_Count] as PC_Normalized_Pricing_Episode_Count_Currrent


	  	  ,p.[PC_Normalized_Pricing_Total_Cost] as PC_Normalized_Pricing_Total_Cost_Previous
      ,c.[PC_Normalized_Pricing_Total_Cost] as PC_Normalized_Pricing_Total_Cost_Currrent


	  	  ,p.[PC_Spec_Episode_Count] as PC_Spec_Episode_Count_Previous
      ,c.[PC_Spec_Episode_Count] as PC_Spec_Episode_Count_Currrent


	  	  ,p.[PC_Spec_Total_Cost] as PC_Spec_Total_Cost_Previous
      ,c.[PC_Spec_Total_Cost] as PC_Spec_Total_Cost_Currrent


	  	  ,p.[PC_Spec_Average_Cost] as PC_Spec_Average_Cost_Previous
      ,c.[PC_Spec_Average_Cost] as PC_Spec_Average_Cost_Currrent


      	  ,p.[PC_Spec_CV] as PC_Spec_CV_Previous
	  ,c.[PC_Spec_CV] as PC_Spec_CV_Currrent


	  ,p.[PC_Spec_Percent_of_Episodes] as PC_Spec_Percent_of_Episodes_Previous
      ,c.[PC_Spec_Percent_of_Episodes] as PC_Spec_Percent_of_Episodes_Currrent

	  ,p.[EC_Spec_Episode_Count] as PC_EC_Spec_Normalized_Pricing_Episode_Count_Previous
      ,c.[PC_Spec_Normalized_Pricing_Episode_Count] as PC_Spec_Normalized_Pricing_Episode_Count_Currrent

	   ,p.[EC_Normalized_Pricing_Total_Cost] as PC_EC_Spec_Normalized_Pricing_Total_Cost_Previous
      ,c.[PC_Spec_Normalized_Pricing_Total_Cost] as PC_Spec_Normalized_Pricing_Total_Cost_Currrent

      
	  ,NULL as PC_Spec_Epsd_Volume_Previous
	  ,c.[PC_Spec_Epsd_Volume] as PC_Spec_Epsd_Volume_Current

	  
	   ,p.[EC_Treatment_Indicator] as EC_Treatment_Indicator_Previous
      ,c.[EC_Treatment_Indicator] as EC_Treatment_Indicator_Currrent


	  ,NULL as EC_Episode_Count_Previous
      ,c.[EC_Episode_Count] as EC_Episode_Count_Current

	   ,p.[EC_Total_Cost] as EC_Total_Cost_Previous
      ,c.[EC_Total_Cost] as EC_Total_Cost_Current

	   ,p.[EC_Average_Cost] as EC_Average_Cost_Previous
      ,c.[EC_Average_Cost] as EC_Average_Cost_Current

	      ,p.[EC_Coefficients_of_Variation] as EC_Coefficients_of_Variation_Previous
      ,c.[EC_Coefficients_of_Variation] as EC_Coefficients_of_Variation_Current

	  ,NULL as EC_Normalized_Pricing_Episode_Count_Previous
      ,c.[EC_Normalized_Pricing_Episode_Count] as EC_Normalized_Pricing_Episode_Count_Current

	  ,NULL as EC_Normalized_Pricing_Total_Cost_Previous
      ,c.[EC_Normalized_Pricing_Total_Cost] as EC_Normalized_Pricing_Total_Cost

	  ,NULL as EC_Spec_Episode_Count_Previous
      ,c.[EC_Spec_Episode_Count] as EC_Spec_Episode_Count_Current

       ,p.[EC_Spec_Total_Cost] as EC_Spec_Total_Cost_Previous
	  ,c.[EC_Spec_Total_Cost] as EC_Spec_Total_Cost_Current

	   ,p.[EC_Spec_Average_Cost] as EC_Spec_Average_Cost_Previous
      ,c.[EC_Spec_Average_Cost] as EC_Spec_Average_Cost_Current

	  ,NULL as EC_Spec_Coefficients_of_Variation_Previous
      ,c.[EC_Spec_Coefficients_of_Variation] as EC_Spec_Coefficients_of_Variation_Current


	    ,p.[EC_Spec_Percent_of_Episodes] as EC_Spec_Percent_of_Episodes_Previous
      ,c.[EC_Spec_Percent_of_Episodes] as EC_Spec_Percent_of_Episodes_Current

	  ,NULL as EC_Spec_Normalized_Pricing_Episode_Count_Previous
      ,c.[EC_Spec_Normalized_Pricing_Episode_Count] as EC_Spec_Normalized_Pricing_Episode_Count_Current

	  ,NULL as EC_Spec_Normalized_Pricing_Total_Cost_Previous
      ,c.[EC_Spec_Normalized_Pricing_Total_Cost] as EC_Spec_Normalized_Pricing_Total_Cost_Current

	   ,Null as EC_CV3_Previous
      ,c.[EC_CV3] as  EC_CV3_Current

	  ,NULL as EC_Spec_Episode_Volume_Previous
      ,c.[EC_Spec_Episode_Volume] as EC_Spec_Episode_Volume_Current
	  
	  ,(CASE WHEN p.Never_Mapped = 0 THEN 'N' ELSE CASE WHEN p.Never_Mapped = 1 THEN 'Y' ELSE NULL END END) as Never_Mapped_Previous
      ,c.[PD_Mapped] as PD_Mapped_Current

      ,NULL as PC_CV3_Previous
	  ,c.[PC_CV3] as PC_CV3_Current

	  ,p.[RX_NRX] as RX_NRX_Previous
      ,c.[RX_NRX] as RX_NRX_Current




  FROM [etg].[VW_ETG_Final_DataLoad] c
  LEFT JOIN (SELECT * FROM [etgsymm].[VW_ETG_Symmetry_Main_Interface] WHERE Data_Period=17 )p ON p.ETG_Base_Class = c.ETG_Base_Class and p.Premium_Specialty = c.Premium_Specialty
