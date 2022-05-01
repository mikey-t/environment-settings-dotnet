const {series} = require('gulp')
const {waitForProcess, defaultSpawnOptions} = require('@mikeyt23/node-cli-utils')
const {spawn} = require('child_process')
const fs = require('fs')
const path = require('path')
require('dotenv').config()

const projPath = './src/MikeyT.EnvironmentSettings'
const packageDir = path.join(projPath, 'bin/Debug')

async function pack() {
  const spawnOptions = {...defaultSpawnOptions, cwd: './src/MikeyT.EnvironmentSettings'}
  await waitForProcess(spawn('dotnet', ['pack'], spawnOptions))
}

async function publish() {
  const packageName = await getPackageName()
  console.log('publishing ' + packageName)
  const spawnOptions = {...defaultSpawnOptions, cwd: packageDir}
  await waitForProcess(spawn('dotnet', [
    'nuget',
    'push',
    packageName,
    '--api-key',
    process.env.NUGET_API_KEY,
    '--source',
    'https://api.nuget.org/v3/index.json'], spawnOptions))
}

async function getPackageName() {
  const csprojPath = path.join(projPath, 'MikeyT.EnvironmentSettings.csproj')
  const csproj = fs.readFileSync(csprojPath, 'utf-8')
  const versionTag = '<PackageVersion>'
  const xmlVersionTagIndex = csproj.indexOf(versionTag)
  const versionStartIndex = xmlVersionTagIndex + versionTag.length
  const versionStopIndex = csproj.indexOf('<', versionStartIndex)
  const version = csproj.substring(versionStartIndex, versionStopIndex)
  return `MikeyT.EnvironmentSettings.${version}.nupkg`
}

exports.publish = series(pack, publish)
