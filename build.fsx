// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.MSBuildHelper
open Fake.XUnit2Helper

let buildDir  = @".\bin\"
let testDir   = @".\binTest\"
let packagesDir = @".\packages"

let project = "Ostrich"
let summary = ".NET port of Twitter's Ostrich library for capturing performance metrics in a CLR app"
let description = ".NET port of Twitter's Ostrich library for capturing performance metrics in a CLR app"
let authors = [ "Madhon" ]
let tags = ""

let solutionFile  = "Ostrich.sln"

let gitOwner = "madhon" 
let gitHome = "https://github.com/" + gitOwner
let gitName = "ostrich"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "CompileApp" (fun _ ->
    !! @"src\**\*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "CompileTest" (fun _ ->
    !! @"tests\**\*.csproj"
      |> MSBuildDebug testDir "Build"
      |> Log "TestBuild-Output: "
)

Target "XUnitTest" (fun _ ->
    !! (testDir + @"\Ostrich.Tests.dll")
      |> xUnit2 (fun p ->
                 {p with ShadowCopy = true; HtmlOutput = true; OutputDir = testDir})
)

Target "NuGet" (fun _ ->
    Paket.Pack(fun p -> 
        { p with
            OutputPath = "dist"
        })
)

Target "PublishNuget" (fun _ ->
    Paket.Push(fun p -> 
        { p with
            WorkingDir = "dist" })
)

Target "All" DoNothing

"All"
  ==> "XUnitTest"

"Clean"
  ==> "CompileApp"
  ==> "CompileTest"
  ==> "XUnitTest"

// start build
RunTargetOrDefault "XUnitTest"