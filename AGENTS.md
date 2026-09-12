## Task scope and completion

- Use the requested outcome and repository constraints to decide what is needed. Read supporting docs and skills only when they apply to the change; a small edit does not require a full repository survey.
- Continue authorized work through implementation and proportionate verification, fixing issues introduced by the change before handing off. Make routine, reversible local decisions without asking again. Ask when missing information materially affects correctness or an action exceeds the authorized scope; identify the specific boundary.
- Choose validation based on risk. Small documentation or low-risk edits do not automatically need builds or tests. For substantial or risky changes, use the smallest relevant checks and rerun affected checks after fixes. Report what was checked and any remaining gaps.

## Test execution

When running specific tests, use Microsoft Testing Platform (MTP) tree node filters.

Do **not** use VSTest filters:

```bash
dotnet test --filter ...
```

Use `--project` to target the test project, then pass MTP arguments after `--`:

```bash
dotnet test --project <project-directory-or-csproj> -- --treenode-filter "<filter>"
```

Filter format:

```text
/<Assembly>/<Namespace>/<Class>/<Test>
```

Examples:

```bash
dotnet test --project ./tests/MyProject.Tests -- --treenode-filter "/*/*/*/MyTest"

dotnet test --project ./tests/MyProject.Tests -- --treenode-filter "/*/*/MyTestClass/*"
```
