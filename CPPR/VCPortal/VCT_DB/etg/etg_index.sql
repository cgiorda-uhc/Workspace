            CREATE INDEX indx_ETG_Episodes_UGAP_PROV_MPIN ON [etg].[ETG_Episodes_UGAP_SOURCE] (PROV_MPIN);
            GO;
                        CREATE INDEX indx_ETG_Episodes_UGAP_ETG_TX_IND ON [etg].[ETG_Episodes_UGAP_SOURCE] (ETG_TX_IND);
            GO;
                        CREATE INDEX indx_ETG_Episodes_UGAP_ETG_BAS_CLSS_NBR ON [etg].[ETG_Episodes_UGAP_SOURCE] (ETG_BAS_CLSS_NBR);
            GO;



                   CREATE INDEX indx_PrimarySpecWithCode_MPIN ON [etg].[PrimarySpecWithCode_PDNDB_SOURCE] (MPIN);
            GO;
                             CREATE INDEX indx_PrimarySpecWithCode_PREM_SPCL_CD ON [etg].[PrimarySpecWithCode_PDNDB_SOURCE] (PREM_SPCL_CD);
            GO;

                               CREATE INDEX indx_PrimarySpecWithCode_NDB_SPCL_CD ON [etg].[PrimarySpecWithCode_PDNDB_SOURCE] (NDB_SPCL_CD);
            GO;

            CREATE INDEX indx_ETG_Mapped_PD_PREM_SPCL_CD ON [etg].[ETG_Mapped_PD_SOURCE] (PREM_SPCL_CD);
            GO;

            
            CREATE INDEX indx_ETG_Mapped_PD_TRT_CD ON [etg].[ETG_Mapped_PD_SOURCE] (TRT_CD);
            GO;

            
            CREATE INDEX indx_ETG_Mapped_PD_ETG_BASE_CLASS ON [etg].[ETG_Mapped_PD_SOURCE] (ETG_BASE_CLASS);
            GO;