# GitHub Configuration
GitHub repositories and projects can be configured with various settings to customize their behavior and functionality.

The `.github` folder in the repository that is used to store configuration files.

## GitHub CLI

Here are the instructions for installing the GitHub CLI on Windows, macOS, and Linux:

### Windows
1. Install [Chocolatey](https://chocolatey.org/install) by following the instructions on the Chocolatey installation page.
2. Open a terminal window (such as the Command Prompt or PowerShell) as an adminstrator and run the following command to install the GitHub CLI using Chocolatey:

```pwsh
choco install gh
```

### macOS
1. Install [Homebrew](https://brew.sh/), a package manager for macOS.
2. Run the following command to install the GitHub CLI using Homebrew:

```zsh
brew install gh
```

### Debian, Ubuntu Linux, Raspberry Pi OS (apt)

```bash
type -p curl >/dev/null || sudo apt install curl -y
curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
&& sudo chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
&& echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
&& sudo apt update \
&& sudo apt install gh -y
```

Install from our package repository for immediate access to latest releases:
```bash
sudo apt update
sudo apt install gh
```

# Branch Protection

Branch protection GitHub allows repository administrators to set policies for how branches can be modified. Branch protection can help ensure that code changes are reviewed and tested before they are merged into the main codebase, and can prevent accidental or unauthorized changes from being pushed to protected branches.

Here are a few reasons why branch protection is important:

 1. Quality control: Branch protection can help ensure that code changes meet certain standards before they are merged into the main codebase. For example, administrators can set policies that require code changes to pass continuous integration tests or to be reviewed by a certain number of reviewers before they can be merged. This can help prevent bugs and other issues from being introduced into the codebase.
 
 2. Collaboration: Branch protection can help facilitate collaboration within a team by requiring that code changes be reviewed and approved by other team members before they are merged into the main codebase. This can help ensure that code changes are consistent with the team's development practices and guidelines.
 
 3. Security: Branch protection can help prevent unauthorized or accidental changes from being pushed to protected branches. This can be especially important for branches that contain critical code or that are used for production deployments.

Overall, branch protection is an important feature that help ensure the quality and security of a codebase and facilitate collaboration within a team.

### Apply branch protection configuration

```bash
gh api --method PUT repos/fuse-digital/quick-setup/branches/master/protection --input .github/branch-protection.json
```
