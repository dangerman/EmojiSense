open System
open System.IO
open System.Collections.Generic
open FSharp.Data
open FSharp.Data.JsonExtensions

let language = "CSharp"

let emojies =
        let dataUrl = "https://raw.githubusercontent.com/github/gemoji/master/db/emoji.json"
        let data = Http.RequestString(dataUrl)
        JsonValue.Parse(data).AsArray()
                |> Seq.filter(fun e -> e.TryGetProperty("emoji") <> None)
                |> Seq.map(fun e -> e?emoji.AsString(), e?aliases.[0].AsString())

let snippetFromEmoji (emoji, description) =
    (String.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<CodeSnippets
    xmlns=""http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet"">
    <CodeSnippet Format=""1.0.0"">
        <Header>
            <Title>{0} {1}</Title>
            <Description>{1} emoji</Description>
            <Shortcut>{1}</Shortcut>
        </Header>
        <Snippet>
            <Code Language=""{2}"">
                <![CDATA[{0}]]>
            </Code>
        </Snippet>
    </CodeSnippet>
</CodeSnippets>", emoji, description, language), emoji, description)

let WriteSnippets snippets =
    let snippetDir = Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\snippets\").FullName
    snippets |> Seq.iter(fun (snippet, emoji, description) -> File.WriteAllText(snippetDir + emoji + @".snippet", snippet))

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    printfn "Creating emoji snippets..."
    emojies |> Seq.map(snippetFromEmoji)
            |> WriteSnippets
    printfn "Done :)"
    0 // return an integer exit code