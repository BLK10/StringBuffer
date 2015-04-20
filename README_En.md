# STRINGBUFFER #
This 'StringBuffer' class has no relation to the equivalent of the same name in Java.

The purpose of this class is to correct some deficiencies of the 'StringBuilder' class

## Why ##

The 'String' class is immutable, every time you use a static method of this class, a new 'String' is created.

So to intensively manipulate text, it is advisable to use the 'StringBuilder' class.

Meanwhile, the 'StringBuilder' class is mutable, but suffer from a lack of functionality, when you need to manipulate text, (cut, replace, etc ...), you have two options:

- Make a conversion to the 'String' class and use it's static methods.

- Create an algorithm to manipulate the characters of the 'StringBuilder' class.

In the two case, you have to do one or more conversions between 'StringBuilder' and 'String' or often create multiple 'String'.

The 'StringBuffer' class responds to this problem.

Then, we have method chaining.
How to control elegantly whether method chaining fail or succeed? (without breaking them)

The 'StringBuffer' class also responds to this problem.

## Implementation ##

So I started the 'StringBuffer' class as an exercise, with certain requirements:

- Reproduce the static methods of the 'String' class and more.
- Support method chaining.
- Never return 'null'.
- Support the two basic types i.e. in addition to the 'StringBuffer' type, the 'String' and the 'Char' type.
- Know whether a method failed silently.

Like the 'StringBuilder', I used a singly linked list, with the difference that I kept a reference of the list's head, and adding and deleting text goes somewhat differently.

All text changes occur at the last moments, the same place, in a group of core methods.
This allows to perfectly control the status of the 'StringBuffer':

- Thus the 'String' representation (via 'ToString()') is cached and is invalidate only if necessary.

- Unlike the 'StringBuilder', we can iterate on a 'StringBuffer' with the 'foreach' statement , but we can't change it.


### Method Chaining ###

I list here, the various chained methods and for each method where it is necessary to pass a string, there is their equivalent overloading for types 'Char', 'String' and 'StringBuffer'.

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
    - FromChar, FromCharArray, FromStringBuffer
    - FormatWith


I tried to make the api consistent.

Two types of formats are supported: the classic format: index based and the named formats.

Some details about methods naming.

There is no 'Substring()' method, its equivalent is 'Crop()'.

There is no 'Replace()' method to replace all the occurrences of a character, its equivalent is 'Substitute()'.

In fact these two methods can not fail silently, that's why i renamed them.

Methods that can fail silently are just methods that begin with 'Replace...' and 'Substring...' because internally they involve to search for a character or a string of characters that returns an index.

If the search fails, the 'StringBuffer is returned as is and is marked (tagged) as failed.

So in a FLUENT way we can monitor the status of the last operation.


### Fluent Interface ###

The two methods to check the status of a 'Replace ...' / 'Substring ...' operation are 'Fail()' and 'Succeed()'.

'Unfail()' method resets the state of the last operation.

Whenever you make a 'Replace ...' / Substring ... ' method call its state is reset, except for the 'Fail()' / 'Succeed()' method where it's state is restored after the execution of the lambda expression.

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


### Method ###

Here, I list the methods that are commonly found in the handling of strings.

    - IndexOf, IndexOfAny
    - LastIndexOf, LastIndexOfAny
    - Contains, ContainsSequence, ContainsAll, ContainsAny
    - StartsWith, StartsWithSequence, StartsWithAny
    - EndsWith, EndsWithSequence, EndsWithAny
    - Split, SplitToBuffer
    - ToCharArray, ToString, Copy
