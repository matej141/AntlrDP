# SqdToOalTranslator
## Zdrojový kód diplomovej práce s názvom _„Generovanie zdrojového kódu z dynamického modelu“_
## Študent: Bc. Matej Čiernik

`SqdToOalTranslator` predstavuje implementované riešenie diplomovej práce s názvom _„Generovanie zdrojového kódu z dynamického modelu“_.

Priečinok [SqdToOalTranslator](SqdToOalTranslator) obsahuje zdrojový kód tejto diplomovej práce.

Podrobnosti o tejto práci sa nachádzajú v jej [textovej verzii](SqdToOalTranslator/Ciernik_Matej_DP.pdf).

Riešenie k diplomovej práci bolo implementované v programovacom jazyku `C#`.


### Spustenie kódu
Kód diplomovej práce je možné spustiť napr. cez súbor [Program.cs](SqdToOalTranslator/Program.cs). Konkrétne sa však dá spustiť týmito príkazmi:
```csharp
var builder = new SqdToOalTranslatorBuilder(selfMessagesAreEmpty: <bool>);
var director = new SqdToOalTranslatorDirector(builder);
director.Construct("<path_to_input_file_defining_sequence_diagram/file_name.json>", "<path_to_animation_output_file/file_name.json >", "<path_to_oal_code_output_file/file_name.txt >");
```
Pre parameter `selfMessagesAreEmpty` triedy `SqdToOalTranslatorBuilder` je možné zvoliť hodnotu `true` alebo `false` podľa toho, či vo výslednom OAL kóde majú byť vlastné správy ako prázdne metódy alebo ako štandardné metódy.

Prvý parameter metódy `Construct()` triedy `SqdToOalTranslatorDirector` určuje cestu k JSON súboru (vrátane názvu súboru) definujúceho sekvenčný diagram v programe `SQD_Tunder`.

Druhým parametrom sa definuje cesta ku výslednému JSON súboru, ktorý po úspešnom vykonaní kódu má byť animačný súbor použiteľný v programe `AnimArch`.

Tretí parameter stanovuje cestu k textovému (txt) súboru súboru, ktorý bude obsahovať text OAL kódu.

### Cesta k súborom
Príklady vstupných súborov definujúcich sekvenčné diagramy sa nachádzajú v priečinku [SqdToOalTranslator/bin/Debug/net7.0/files](SqdToOalTranslator/bin/Debug/net7.0/files).<br>
Súbory s OAL kódmi, spomínané v kapitole o evaluácii v [texte práce](SqdToOalTranslator/Ciernik_Matej_DP.pdf), sa nachádzajú v priečinku [SqdToOalTranslator/bin/Debug/net7.0/files/evaluation_files](SqdToOalTranslator/bin/Debug/net7.0/files/evaluation_files).

### Časti zdrojového kódu
Jednotlivé časti zdrojového kódu, spomínané v kapitole _2.1.3_ v [texte práce](SqdToOalTranslator/Ciernik_Matej_DP.pdf), sa nachádzajú v priečinkoch [SequenceDiagramElements](SqdToOalTranslator/SequenceDiagramElements), [PreOalCodeElements](SqdToOalTranslator/PreOalCodeElements) a [Translation](SqdToOalTranslator/Translation).

Diagram tried nášho riešenia sa nachádza na samostatnom [obrázku](SqdToOalTranslator/class_diagram_complete.jpg). V tomto diagrame sú zobrazené iba najzákladnejšie parametre a metódy daných tried. Nenachádzajú sa v ňom napr. pomocné triedy, pomocné metódy, či atribúty.

Triedy implementované podľa návrhového vzoru _Builder_, ktorými sa spúšťa náš kód, sa nachádzajú v priečinku [Builder](SqdToOalTranslator/Builder).

V priečinku [AntlrFiles](SqdToOalTranslator/AntlrFiles) sa nachádzajú všetky potrebné súbory, na základe ktorých sa spúšťa nástroj `ANTLR`. Nami prispôsobený _Visitor_ tohto nástroja, ktorým sa zároveň začína vykonávanie generovania OAL kódu, sa nachádza v súbore [SequenceDiagramCustomVisitor.cs](SqdToOalTranslator/SequenceDiagramCustomVisitor.cs).

### Testy
Jednotkové testy, ktorými sme postupne testovali jednotlivé časti zdrojového kódu nášho riešenia, sa nachádzajú v súboroch [SequenceDiagramTests.cs](SqdToOalTranslatorTests/SequenceDiagramTests.cs), [PreOalCodeTests.cs](SqdToOalTranslatorTests/PreOalCodeTests.cs) a [TranslatorTests.cs](SqdToOalTranslatorTests/TranslatorTests.cs). Tieto súbory sú v priečinku [SqdToOalTranslatorTests](SqdToOalTranslatorTests).

JSON súbory definujúce sekvenčné diagramy, ktoré využívajú naše testy, sa nachádzajú v priečinku [SqdToOalTranslatorTests/bin/Debug/net7.0/files](SqdToOalTranslatorTests/bin/Debug/net7.0/files).

