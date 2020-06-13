import json, codecs
import numpy as np
from nltk import ngrams

question_hist = []
commonWords = ['<instrucción>','<expresión>','if','then','hay','Cual','e','la', 'las', 'el', 'lo','los' ,'al', 'no' , 'si', 'se', 'es', 'de', 'del' , 'y' , ''
               'como', '#','20+5*30', '*','**','---','=','==','===>','=_===>_==>','=====>','==>' ,'=>#',
               '<>','====#','a', 'a*b**c+d', 'a*bc|d+', 'a+b*' ,'a+b+c' ,'a:' ,'a=b+c;','[+|-','[else', ']','La', 'Las',
               'hasta','“:”' ,'“;”' ,'“==>”', '“=”' ,'“_”', 'son','su','son:','“”','cada',
                'a','ante','bajo','cabe','con','contra','de','desde','en','entre','hacia','hasta','para','por','según','sin','so','sobre','tras',
                '', '*/', '-x+x*x', '/' ,'/*', '10;' ,'11-' ,'40' ,'5', '7-' ,'8-','ab*c|d+','ac*b|c+', 'c', 'co','En','El','un','Un', 'en',
               'esta','este','está', 'han', 'ha', 'que', 'qué', 'una', 'Cuál','Cual','Cuántas', 'LOOS'
               ]

def intersection(hist_dataset, hist_im_query):
    """
        Function that calculates the n intersection distances from a test image histogram data
        to n histograms in the dataset
    Args:
        hist_dataset: Matrix containing the n dataset histograms data
        hist_im_query: Matrix containing the test image histogram data
    Returns: Matrix containing n intersection distances
    """

    closests = np.argsort(1/((np.sum(np.minimum(hist_dataset, hist_im_query), axis=1)) + 0.0000001))
    return closests
  

with codecs.open('Database.json', 'r+', encoding='utf-8') as file:
    data = json.load(file)
    
    n = 2

    
    words = {word: 0 for value in data["preguntas"].values() for word in ngrams(sorted(list(set(value["texto"].split()) - set(commonWords))), n)}
    
    #for k in words.keys():
    #  if k in commoWords:
    #    del(words[k])
        
        
    #n = 6
	#sixgrams = ngrams(value["texto"].split(), n)
    #value["texto"].split()
    
    for v in data["preguntas"].values():
        
        #for word in v["texto"].split():
        for word in ngrams(sorted(list(set(v["texto"].split()) - set(commonWords))), n):
            words[word] += 1
        
        question_hist.append([*words.values()])
        
        words = dict.fromkeys(words, 0)

question_hist = np.array(question_hist)


for i, (key, value) in enumerate(data["preguntas"].items()):

    hist = question_hist[i]
    
    closests = intersection(question_hist, hist)
    
    data["preguntas"][key]["cercanas"] = np.array([*data["preguntas"].keys()])[closests[:6]].tolist()
    if key in data["preguntas"][key]["cercanas"]:
        data["preguntas"][key]["cercanas"].remove(key)

    print(key, data["preguntas"][key]["cercanas"])
    print(data["preguntas"][key]["texto"])
    print(data["preguntas"][[*data["preguntas"][key]["cercanas"]][0]]["texto"])
    print("------------------------------------------")

with codecs.open('DatabaesModified.json', 'w', encoding='utf-8') as fileDos:
	json.dump(data, fileDos, indent=4, ensure_ascii=False)