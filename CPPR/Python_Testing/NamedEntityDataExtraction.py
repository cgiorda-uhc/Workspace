"""
Named Entity Recognition (NER)
DOWNLOAD PREMADE MODELS VIA Terminal Window:

C:\Python36x64\python.exe -m spacy download en

C:\Python36x64\python.exe -m pip install --upgrade pip

C:\Python36x64\python.exe -m spacy download en_core_web_lg
C:\Python36x64\python.exe -m spacy download en_core_web_sm
C:\Python36x64\python.exe -m spacy download en

C:\Python36x64\python.exe -m spacy link en_core_web_sm en

indent error find!
C:\Python36x64\python.exe -m tabnanny NamedEntityDataExtraction.py
"""
#import scispacy
#from scispacy.umls_linking import UmlsEntityLinker
#import spacy.matcher as phrase_matcher
import spacy
from negspacy.negation import Negex
import plac
import  pathlib as Path
import random


model_name_global = "en_core_web_sm"
model_name_global = "C:\Python36x64\Lib\site-packages\en_core_med7_lg\en_core_med7_lg-0.0.2"


def med7_test():
    med7 = spacy.load(model_name_global)
    #linker = UmlsEntityLinker()
    # this can take a while the first time
    #med7.add_pipe(linker)
    # create distinct colours for labels
    col_dict = {}
    seven_colours = ['#e6194B', '#3cb44b', '#ffe119', '#ffd8b1', '#f58231', '#f032e6', '#42d4f4']
    for label, colour in zip(med7.pipe_labels['ner'], seven_colours):
        col_dict[label] = colour

    #options = {'ents': med7.pipe_labels['ner'], 'colors': col_dict}

    text = 'A patient was prescribed Magnesium hydroxide 400mg/5ml suspension PO of total 30ml bid for the next 5 days. Patient should not take asprin or lipitor. Patient cant take ambien'
    doc = med7(text)
    ents = [(x.text, x.label_) for x in doc.ents]
    print(ents)

    negex = Negex(med7, ent_types=["DRUG"])
    med7.add_pipe(negex, last=True)
    doc = med7(text)
    for e in doc.ents:
        print(e.text, e._.negex)
    #print(e.text, e._.negex, e._.umls_ents)

    #spacy.displacy.render(doc, style='ent', jupyter=True, options=options)

    #[(ent.text, ent.label_) for ent in doc.ents]

def find_entity( str_text=""):
    nlp = spacy.load(model_name_global)
    doc= nlp(str_text)
    ents = [(x.text, x.label_) for x in doc.ents]
    print(str_text)
    print(ents)


def linguistic_annotations( str_text=""):
    nlp = spacy.load(model_name_global)
    doc= nlp(str_text)
    for token in doc:
        print(token.text, token.pos_, token.dep_)

def tokenization( str_text=""):
    nlp = spacy.load(model_name_global)
    doc= nlp(str_text)
    for token in doc:
        print(token.text)

def pocess_text( str_text=""):
    nlp = spacy.load(model_name_global)
    doc= nlp(str_text)
    cleaned = [y for y in doc if not y.is_stop and y.pos_ != 'PUNCT']
    raw = [(x.lemma_, x.pos_) for x in cleaned]
    print(str_text)
    print(raw)


"""
def ner_test():
    nlp = spacy.load(model_name_global)
    if "ner" not in nlp.pipe_name:
        ner = nlp.create_pipe("ner")
        nlp.add_pipe(ner)
    else:
        ner = nlp.get_pipe("ner")
    label = "CIADIR"
    matcher = phrase_matcher(nlp.vocab)
    for i in ['Gina Haspel', 'Gina', 'Haspel', ]:
        matcher.add(label, None, nlp(i))
    print(ner)



def offseter(lbl, doc, matchitem):
  o_one = len(str(doc[0:matchitem[1]]))
  subdoc = doc[matchitem[1]:matchitem[2]]
  o_two = o_one + len(str(subdoc))
  return  o_one, o_two, lbl
"""


