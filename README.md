# CoursesDownloader-CSharp

~~Same~~ as [CoursesDownloader-Python](https://github.com/anton31kah/CoursesDownloader-Python) but in C#. Well it was the same, until I started adding to it, now it's way bigger, more powerful, and better.

This script is a simple downloader for the website http://courses.finki.ukim.mk/, it provides an automated way to download multiple files, it has some actions, and is constantly expanded, many features will be introduced in the future, but keep in mind that the purpose of this project was to automate the process so it should make it faster and easier and not slower and harder.

Download latest version from [here](https://github.com/anton31kah/CoursesDownloader-CSharp/releases/latest).

### Features:
- Select files to download with patterns.
    - Example: `1-5,7,9,11-15` will download files `1,2,3,4,5,7,9,11,12,13,14,15`.
- Smart and flexible checks on previously mentioned selections, so if files 14,15 aren't in range, it'll ask whether to ignore them or download something else (entirely or just to replace those out of range).
- Powerful actions like:
  - **Add**: Add anything to the queue, files, sections, even whole courses, using the previous patterns, so yes you can download the whole semester at once.
  - **Remove**: Remove what you added, applies to the same categories, and it doesn't complain whether the items were added.
  - **Queue**: Show what will be downloaded in pages, you can **remove** from here as well, following the same patterns, or **clear** them all if you change your mind, or **download** the queue.
  - **Back**, **Home**: Navigate around the console, home takes you to the main courses selection screen, while back goes one step up the tree.
  - **Copy**, **Open**: On links or anywhere (will be applied to the parent selection), copy or open the links you selected, either to a course or specific section, or even files.
  - **Exit**, **Quit**, **Close**; Self-explanatory.
  - **Refresh**: Refresh the course you're in.
  - **Download**: Starts downloading the download queue.
  - **SwitchSemester**: Switch to an old semester.
  - **TempUserLogIn**, **TempUserLogOut**: Log in or out as a temp user, the credentials will be deleted safely after log out (or after console close, everything is disposed in whatever situation).
  - **LogOut**: Log out and be prompted to enter credentials again, any stored credentials will be disposed.
  - And more to come.
- Downloads external links as urls, pages as pdfs, do not miss anything while downloading.

### Plans for the future:
- [x] Download external links as
    - [ ] ~~Pdf using athenapdf.~~
    - [x] Link shortcuts.

- [x] Actions to implement:
  - [x] **DownloadQueue**: You prepared the download queue, now what? yup, sorry for forgeting this important action but it's late now. This action will start downloading the queue.
  - [x] **SwitchSemester**: You ever need to restudy some old materials? or just want to share them with younger colleagues? This action will let you go switch to previous semesters.
  - [x] **Add**: Adds links to download queue (selected_links), either previously selected links (when in choose naming method) if no numbers are given, or if numbers given then add those, if called on a section or course, then recursivly everything inside that is added.
  - [x] **Queue**: Show download queue (selected_links) (think of a better command name).
  - [x] **Clear**: Can clear the download queue, or if in course or section, removes those from download queue, if in naming method, removes those selected.
  - [x] **Remove**: Accepts numbers, can remove links from download queue, or if in course or section, removes selection from download queue, if in naming method, removes those selected.
  - [x] **Home**: Takes a user home to all courses.

- [x] Make the project cross platform, although this step is harder than it seems, mainly struggling to find a linux credential manager like the windows one or the keychain in mac os, if anyone knows something like that, create a new issue and tell me.

- [x] When a section is opened, do async get on all links inside, this gets the urls without the content, from there get filename (maybe through headers) and store both file names (from courses and url), the async get should be a future object, it should run in background but if requested explicitly then is fetched immediatelly (with wait), this allows name prediction.

- [ ] ~~Maybe automatically download all links in background once added to queue, then prevent the user from closing the console by catching the close [like this](https://stackoverflow.com/a/5334683/6877477).~~
