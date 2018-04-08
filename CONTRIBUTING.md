# Start9 Contributing Guidelines *WIP*
Thanks for taking the time to contribute to Start9! To make sure that your efforts are used effectively, please take the time to read over these guidelines.

## Submiting an issue 
We use GitHub issue tracking to manage problems and suggestions with Start9. If you've found a bug in Start9, this is where you should go. The only prerequisite for submiting a problem or suggestion is to [create a free GitHub account](https://github.com/join). Creating a GitHub account is free and takes minutes.   
*The issues page can be found here:* 

![A screenshot of a mouse pointer hovering over the link for the GitHub issues page.](https://i.imgur.com/xfGsft6.png)

### Security Issue 
- **Do not report the issue using GitHub**
- Contact a Core Developer privately on [Discord](https://discord.gg/6cpvxBS) in a [responsible manner](https://en.wikipedia.org/wiki/Responsible_disclosure).

### Non-security related isuse 
1. Please check if the issue has been already posted on the issues page 
3. Include the exact version of Start9 you are using 
4. Use a clear title and description 
5. Add steps on how to reproduce your issue 
6. Add the "bug" label 

## Submitting a suggestion 
The easiest way to have your suggestion implemented into Start9 quickly is for you or someone else write code and create a pull request. However, you may also suggest ideas using GitHub issues tracking, although it may take much longer for a feature to be added. See "[Writing Code](#writing-code)."
1. Include a clear title and description of what you want in Start9
2. Add concepts or mockups if needed
3. Add the "suggestion" label

## Writing Code 
Please save us time by complying to our code guidelines.

### Code style 
In general, just respect the rules that are used in C# in other places. You can see the C# design guidelines [here](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions). To summarize:

**Do:** 
- Allman style bracketing.
- Everything uses `PascalCase`.
	- Exceptions: Parameters and locals use `camelCase`. Private instance fields use `_camelCase.`
- Use `var` only if the type is apparent.

However, we ask that you use tabs instead of spaces.

**Don't:** 
- Write hard to read code 
- Refuse to write documentation for the code you write for Start9 
- Spam pull requests if your first one was rejected, unless you've fixed the problem pointed out. 

### Comments 
Please comment on all your code, including functions, using XML documentation. Fill out as much information about the member or type as you can.

<!-- ## Translating Start9 
Localization is important for Start9 to grow! Here's how you can help. 
**SECTION TBD**: for now, use #start9 on the [Start9 Discord](https://discord.gg/6cpvxBS). -->

## Feedback 
Would you want to improve these guidelines? Please edit this guide with your proposed changes and submit a pull request.
