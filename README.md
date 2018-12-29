# CoursesDownloader-CSharp

Same as [CoursesDownloader-Python](https://github.com/anton31kah/CoursesDownloader) but in C#

This script is a simple downloader for the website http://courses.finki.ukim.mk/, it provides an automated way to download multiple files, it has some actions, and is constantly expanded, many features will be introduced in the future, but keep in mind that the purpose of this project was to automate the process so it should make it faster and easier and not slower and harder.

This script depends on a few packages that I'll list here in the future. 

Todo:
- [ ] When a section is opened, do async get on all links inside, this gets the urls without the content, from there get filename (maybe through headers) and store both file names (from courses and url), the async get should be a future object, it should run in background but if requested explicitly then is fetched immediatelly (with wait), this allows name prediction.

- [ ] Actions to implement:
	- [ ] Add: adds links to download queue (selected_links), either previously selected links (when in choose naming method) if no numbers are given, or if numbers given then add those, if called on a section or course, then recursivly everything inside that is added.
	- [ ] Queue: show download queue (selected_links) (think of a better command name).
	- [ ] Clear: can clear the download queue, or if in course or section, removes those from download queue, if in naming method, removes those selected.
	- [ ] Remove: accepts numbers, can remove links from download queue, or if in course or section, removes selection from download queue, if in naming method, removes those selected.
	- [ ] Home: takes a user home to all courses.

- [ ] Maybe automatically download all links in background once added to queue, then prevent the user from closing the console by catching the close [like this](https://stackoverflow.com/a/5334683/6877477).

- [ ] Download external links as pdf using athenapdf, maybe use selenium, store those links in a txt file and create link shortcuts.
