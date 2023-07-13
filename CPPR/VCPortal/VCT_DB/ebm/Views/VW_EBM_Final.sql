CREATE VIEW [ebm].[VW_EBM_Final]
	AS 
    SELECT 
       d.[REPORT_CASE_ID],
      d.[REPORT_RULE_ID],
      d.[COND_NM],
      d.[RULE_DESC],
      d.[PREM_SPCL_CD],
      d.[CNFG_POP_SYS_ID],
      d.[LOB],
      d.[MKT_NBR],
      d.[UNET_MKT_NBR],
      d.[UNET_MKT_DESC],
      d.[Current_Version],
      d.[Current_Market_Compliant],
      d.[Current_Market_Opportunity],
      d.[Current_National_Compliant],
      d.[Current_National_Opportunity],
      d.[Previous_Version],
      d.[Previous_Market_Compliant],
      d.[Previous_Market_Opportunity],
      d.[Previous_National_Compliant],
      d.[Previous_National_Opportunity],
      d.[DTLocation],
      d.[Data_Extract_Dt],
	  r.[MKT_NM],
	r.[MAJ_MKT_NM],
	r.[RGN_NM],
	r.[MKT_RLLP_NM],
  r.[RRLocation]
  FROM [ebm].[DQC_DATA_UHPD_SOURCE] d
  LEFT JOIN [vct].[Rate_Region] r ON r.[MKT_NBR] = d.[MKT_NBR]