// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.MSBuildHelper
open Fake.XUnit2Helper

let buildDir  = @".\bin\"
let testDir   = @".\bin\"
let packagesDir = @".\packages"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir]
)

Target "CompileApp" (fun _ ->
    !! @"Ostrich\**\*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "CompileTest" (fun _ ->
    !! @"Ostrich.Tests\**\*.csproj"
      |> MSBuildDebug testDir "Build"
      |> Log "TestBuild-Output: "
)

Target "XUnitTest" (fun _ ->
    !! (testDir + @"\Ostrich.Tests.dll")
      |> xUnit2 (fun p ->
                 {p with ShadowCopy = true; HtmlOutput = true; OutputDir = testDir})
)

// Dependencies
"Clean"
  ==> "CompileApp"
  ==> "CompileTest"
  ==> "XUnitTest"

// start build
RunTargetOrDefault "XUnitTest"