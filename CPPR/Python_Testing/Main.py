from NamedEntityDataExtraction import pocess_text, linguistic_annotations, tokenization,find_entity,med7_test
from DBConnection import get_data_table


med7_test()

#str_text = "Hello my name is Chris Giordano and today I am trying to learn Spacy. I live in New Jersey in the town of Florence"

#find_entity(str_text)

#str_text = "This is a test and hopefully it will have the coolest results ever!!!"

#pocess_text(str_text)
#linguistic_annotations(str_text)
#tokenization(str_text)

"""
df = get_data_table("Driver={SQL Server};Server=wn000005325;Database=IL_UCA;Trusted_Connection=yes;", "SELECT TOP 10 * FROM  covid19_pdf_mbr")

for label, row in df.iterrows():
    print(label)
    print(row)

for label, row in df.iterrows():
    print(str(label) + " -:- " + row["pdf_folder"])
"""