# STRINGBUFFER #

Cette classe 'StringBuffer' n'a aucun rapport avec son équivalent du même nom en Java.

Le but de cette classe est de corriger certaine lacune de la classe 'StringBuilder'.

## Pourquoi ##
La classe 'String' est immuable, chaque fois que vous utiliser une méthode statique de cette classe, une nouvelle chaîne de caractère est crée.

Donc, pour manipuler du texte de manière intensif, il est conseiller d'utiliser la classe 'StringBuilder'.

La classe 'StringBuilder' quant à elle, est muable, mais elle souffre d'un cruel manque de fonctionnalité, des que vous devez manipuler du texte, (couper, remplacer, etc...) vous avez deux possibilités:

- Faire une conversion vers la classe 'String' est utiliser les méthodes statiques.

- Créer un algorithme de manipulation de caractères a partir de la classe 'StringBuilder'.

Dans les deux cas, vous devez faire une ou plusieurs conversions entre un 'StringBuilder' et un 'String' ou souvent créer plusieurs 'String'.

La classe 'StringBuffer' répond a ce problème.

Ensuite, nous avons les méthodes chaînées.

Comment controler l'echec ou la réussite d'une méthode chaîné élégamment? (c-a-d sans briser les méthodes chaînés).

La classe 'StringBuffer' répond également a ce problème.


## Mise en oeuvre ##

J'ai donc commencé la classe 'StringBuffer' comme un exercice, avec certains impératifs:

- Reproduire les méthodes statiques de la classe 'String' et plus.
- Supporter le chaînage de méthodes.
- Ne jamais retourner 'null'.
- Supporter les 2 types de bases c-a-d en plus du type 'StringBuffer', le type 'String' et 'Char'.
- Savoir si une méthode a échoué silencieusement.

Tout comme le 'StringBuilder', je suis donc partie sur une liste simplement chaînée, à la différence près que je garde une référence de la tête de liste, et que l'ajout et la suppression de texte se passe quelque peu différemment.

Toutes les modifications de textes interviennent aux derniers moments, aux même endroit, dans un groupe de méthodes de base (comprendre 'core methods').
Cela permet de contrôler parfaitement l'état du 'StringBuffer':

- Ainsi la représentation sous forme de 'String'(via 'ToString()') est mis en cache et est invalider uniquement si nécessaire.

- Contrairement au 'StringBuilder', on peut itérer sur un 'StringBuffer' à l'aide de la déclaration 'foreach', mais on ne peut le modifier.


### Méthode Chainée ###

Je liste ici les différentes méthodes chaînées et pour chaque méthodes ou il est nécessaire de passer une chaîne de caractère, il existe leurs équivalent pour les types 'Char', 'String' et 'StringBuffer'. 

	- Clear
    - Append, AppendLine, AppendFormat, AppendFormatLine
    - Prepend, PrependLine, PrependFormat, PrependFormatLine
    - Insert, InsertLine, InsertFormat, InsertFormatLine
    - Crop, Remove, Substitute
    - Replace, ReplaceRange, ReplaceBefore, ReplaceAfter, ReplaceInside
    - SubstringRange, SubstringBefore, SubstringAfter, SubstringInside, SubstringOutside
    - Trim, TrimStart, TrimEnd
    - PadLeft, PadRight
    - ToLower, ToLowerInvariant, ToUpper, ToUpperInvariant
    - FromChar, FromCharArray, FromString, FromStringBuffer
    - FormatWith


J'ai essayé de faire en sorte que l’api soit cohérent.

Deux types de formats sont supportés: le format classique: basé sur un index et les formats nommés.

Quelques précisions sur le nommage des méthodes.

Il n'existe pas de méthode 'Substring()' son équivalent est 'Crop()'.

Il n'existe pas de méthode 'Replace()' pour remplacer toutes les occurrences d'un caractère, son équivalent et 'Substitute()'

En effet ces deux méthodes ne peuvent pas échouer silencieusement, C'est pourquoi je les ai renommé.

Les méthodes qui peuvent échouer silencieusement sont les méthodes commençant justement par 'Replace...' et 'Substring...' car en interne elles font appel à la recherche d'un caractère ou d'une chaîne de caractère qui retourne un index.

Si la recherche échoue, le 'StringBuffer' est retourné tel quel et est marqué(tagué) comme ayant échoué.

Ainsi d'une manière FLUIDE on peut contrôler l'état de la dernière opération. 


### Interface Fluide ###

Les deux méthodes pour contrôler l'état d'une opération 'Replace...' / 'Substring...' sont 'Fail()' et 'Succeed()'.

La méthode 'Unfail()' réinitialise l'état de la dernière opération.

A chaque fois que vous faite appel à une méthode 'Replace...' / Substring...' son état est réinitialisé, à part si cela ce passe dans une méthode fluide, ou son état est restitué après l’exécution de l'expression lambda.

    - Fail
    - Succeed
    - Unfail
    - Do
    - While
    - DoWhen
    - DoAtWhen
    - DoAtWhenElse
    - DoWhenElse
    - DoWhile
    - For


### Méthode ###

Ici, je liste les méthodes que l'ont retrouve couramment dans la manipulation de chaînes de caractères.

    - IndexOf, IndexOfAny
    - LastIndexOf, LastIndexOfAny
    - Contains, ContainsSequence, ContainsAll, ContainsAny
    - StartsWith, StartsWithSequence, StartsWithAny
    - EndsWith, EndsWithSequence, EndsWithAny
    - Split, SplitToBuffer
    - ToCharArray, ToString, Copy

