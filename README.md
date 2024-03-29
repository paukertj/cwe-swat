# CWE SWAT

Content repo for CWE SWAT Team

## Shorts
Shorts are short articles up to 5 mins to read. Put them into `.\shorts` folder. Plase name them by folowing pattern

```
PUBLISH-DATE_AUTHOR-ALIAS_SHORT-DESCRIPTION.md
```
#### PUBLISH-DATE_AUTHOR
Date when content was published in format `YYYY-MM-DD`

#### AUTHOR-ALIAS
CWE alias of publishing author (usually also creator) (f.e. `JPA`)

#### ALIAS_SHORT-DESCRIPTION
Short description (f.e. `Linq-Where-Complexity`, `Loop-Benchmark`, ...)

#### Example
Consider post publised on 2024-02-05 about LINQ where complexity created and published by JPA.
```
2024-02-05_JPA_Linq-Where-Complexity.md
```

### Template
Each post should has following structure:
- Introduction
    - Do you know that?
    - I want to introduce you the new feature...
    - I've found interesting...
- Actual content
- Suffix
    - You can create your own
    - You can use the one form Suffix section

### Suffix
```
> Found a bug or have additional questions? Let me know in the comments! I created this post on behalf of the CWE [**SWAT Workgroup**](https://wiki.ciklum.net/display/CGNA/SWAT+Workgroup). You can reach me and other group members at swat@ciklum.com.
```

## Blog posts
Blog post that explains some topic. Put them into `.\blog-posts` folder.

## Creator rules
- Use markdown format
- Please do one commit into `main` branch with one content item. F.e. if you wirte one shorts post, please do it in one commit
- If you need branching, use your CWE alias as branch prefix. F.e. `JPA/my-super-interesting-blog-post`
