Framework "4.0"
properties {
    $solution = '.\EventMachine.sln'
    $version = '1.0.0'
}

task default -depends Build

task Clean {
    msbuild $solution /t:Clean
}

task Build {
    msbuild $solution /property:Configuration=Release
}
