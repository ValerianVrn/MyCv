import os
import sys
import openai
from pathlib import Path
import subprocess
from github import Github

issue_number = sys.argv[1]
command = sys.argv[2]

openai.api_key = os.environ["OPENAI_API_KEY"]

# Read repo files (keep it lightweight – avoid huge binaries)
repo_files = {}
for path in Path(".").rglob("*.*"):
    if ".git" in path.parts or path.stat().st_size > 200_000:
        continue
    try:
        repo_files[str(path)] = path.read_text()
    except Exception:
        pass

prompt = f"""
You are an expert C# developer and MudBlazor designer.
The repo files are: {list(repo_files.keys())}

Instruction: {command}

Update the files accordingly. Return the full updated content for each file in the format:

=== filename ===
<content>
"""

# Call OpenAI
response = openai.ChatCompletion.create(
    model="gpt-4",
    messages=[{"role": "user", "content": prompt}],
    temperature=0.7
)

ai_output = response.choices[0].message.content

# Parse AI output and write changes
import re
changed_files = []
for match in re.finditer(r"=== (.+?) ===\n(.*?)(?=\n===|\Z)", ai_output, re.S):
    filename, content = match.groups()
    Path(filename).write_text(content)
    changed_files.append(filename)

# Create new branch
branch_name = f"ai-update-{issue_number}"
subprocess.run(["git", "config", "--global", "user.name", "AI Bot"])
subprocess.run(["git", "config", "--global", "user.email", "ai@bot.com"])
subprocess.run(["git", "checkout", "-b", branch_name])
subprocess.run(["git", "add"] + changed_files)
subprocess.run(["git", "commit", "-m", f"AI update: {command}"])
subprocess.run(["git", "push", "origin", branch_name])

# Create Pull Request
g = Github(os.environ["GITHUB_TOKEN"])
repo = g.get_repo(os.environ["GITHUB_REPOSITORY"])
issue = repo.get_issue(int(issue_number))
pr = repo.create_pull(
    title=f"AI Update: {command}",
    body=f"AI-generated update from Issue #{issue_number}",
    head=branch_name,
    base="main"
)
issue.create_comment(f"Pull Request created: {pr.html_url}")