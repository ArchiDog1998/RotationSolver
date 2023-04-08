const user = "ArchiDog1998";
const repos = [
    "RotationSolver",
    "FakeName",
];

const clearText = (str) => {
  return str
    .replace(/\p{Extended_Pictographic}/gu, "") // remove emojis
    .replace(/\*\*(.*)\*\*/g, "$1") // extract markdown bold text
    .replace(/\[([^\)]+)\]\([^\)]+\)/g, "$1") // extract markdown link label
    .split(/\r?\n/g)
    .map(line => line.replace(/^#+\s+/g, ""))
    .join("\n");
};

const output = await Promise.all(repos.map(async (repo) => {
  const res = await fetch(`https://api.github.com/repos/${user}/${repo}/releases/latest`);
    const data = await res.json();
    const reses = await fetch(`https://api.github.com/repos/${user}/${repo}/releases`);
    const datas = await reses.json();

    var count = 0;
    for (var i = 0; i < datas.length; i++) {
        count = count + Number(datas[i].assets[0].download_count);
    }

  const base = {
    AssemblyVersion: data.tag_name.replace(/^v/, ""),
      //Changelog: clearText(data.body),
      DownloadCount: count.toString(),
      LastUpdate: new Date(data.published_at).valueOf() / 1000,
      RepoUrl: `https://github.com/${user}/${repo}`,
    DownloadLinkInstall: data.assets[0].browser_download_url,
    DownloadLinkUpdate: data.assets[0].browser_download_url,
  };

    const manifestRes = await fetch(`https://raw.githubusercontent.com/${user}/${repo}/main/manifest.json`);
    if (!manifestRes.ok) {
        return Object.assign({
            Author: user,
            Name: repo,
            InternalName: repo,

            ApplicableVersion: "any",
        }, base);
    }
  const manifest = await manifestRes.json();
  return Object.assign(manifest, base);
}));

await Deno.writeTextFile("pluginmaster.json", JSON.stringify(output, null, 2));