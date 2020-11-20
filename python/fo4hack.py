import sys

class Comp:
    def __init__(self, name, likenesses):
        self.name = name
        self.likenesses = likenesses

class Word:
    comparisons = []
    distinction = 0
    deviation = 0

    def __init__(self, name):
        self.name = name

    def compare(self, toCompare):
        return len(list(filter(lambda tup: tup[0] == tup[1], zip(self.name, toCompare.name))))

    def buildComparisons(self, words):
        self.comparisons = list(map(lambda w: Comp(w.name, self.compare(w)), words))
        
        distinct = set()
        for l in map(lambda c: c.likenesses, self.comparisons):
            if not l in distinct:
                distinct.add(l)
                #yield l
        self.distinction = len(distinct)
        print(self.distinction)

        #TODO set deviation

    def prune(self, names):
        print(" ".join(names))
        self.comparisons = list(filter(lambda comp: comp.name in names, self.comparisons))
        print("after"+str(len(self.comparisons)))

possibles = []
while True:
    word = input(">> ").strip()
    if word == "":
        break
    if len(possibles) > 0:
        if len(possibles[0].name) < len(word):
            print("Too Long!")
            continue
        elif len(possibles[0].name) > len(word):
            print("Too Short!")
            continue
    possibles.append(Word(word))

for left in possibles:
    print("build possibles")
    left.buildComparisons(possibles)

while True:
    possibles.sort(key=lambda w: w.deviation)
    possibles.sort(key=lambda w: w.distinction, reverse=True)

    report = "{name} : {distinction}|{deviation:.2f} : {likenesses}"
    for word in possibles:
        print(report.format(name=word.name, distinction=word.distinction, deviation=word.deviation, likenesses=('|'.join(map(lambda c: str(c.likenesses), word.comparisons)))))

    best = possibles[0]
    print('Best of {count}: "{name}"'.format(count=len(possibles), name=best.name))

    likenesses = int(input("Likenesses: "))

    matches = list(map(lambda comp: comp.name, filter(lambda comp: comp.likenesses == likenesses, best.comparisons)))
    possibles = list(filter(lambda w: w.name in matches, possibles))

    if len(possibles) == 1:
        print('The passkey is: "{name}"'.format(name=possibles[0].name))
        print("Another successful hack!")
        sys.exit(0)

    for w in possibles:
        w.prune(matches)

        