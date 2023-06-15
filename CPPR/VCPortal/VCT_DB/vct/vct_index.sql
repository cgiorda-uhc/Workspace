            CREATE INDEX indx_ETG_Episodes_UGAP_PROV_MPIN ON [vct].[ETG_Episodes_UGAP] (PROV_MPIN);
            GO;
                        CREATE INDEX indx_ETG_Episodes_UGAP_ETG_TX_IND ON [vct].[ETG_Episodes_UGAP] (ETG_TX_IND);
            GO;
                        CREATE INDEX indx_ETG_Episodes_UGAP_ETG_BAS_CLSS_NBR ON [vct].[ETG_Episodes_UGAP] (ETG_BAS_CLSS_NBR);
            GO;



                   CREATE INDEX indx_PrimarySpecWithCode_MPIN ON [vct].[PrimarySpecWithCode] (MPIN);
            GO;
                             CREATE INDEX indx_PrimarySpecWithCode_PREM_SPCL_CD ON [vct].[PrimarySpecWithCode] (PREM_SPCL_CD);
            GO;

                               CREATE INDEX indx_PrimarySpecWithCode_NDB_SPCL_CD ON [vct].[PrimarySpecWithCode] (NDB_SPCL_CD);
            GO;