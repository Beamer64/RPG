# RPG
## Text/GUI based RPG game

This is a small RPG-like game that was built on the foundation of various online lessons and is now used for practice and developed as a hobby.

This game gives you the option to play in two separate play styles. One play style has a GUI and visual components for things such as inventory and quests. The other is a Text Adventure style RPG similar to a personal favorite ZORK. The Text version will allow you to maneuver and interact with the area via command interpretations.

Both games save progress automatically as well as carry over progress from one style to the next. So if you’d like to start in the GUI and later progress using the Text version, all of your saved progress will transfer, unless you choose to start a new game.

The game uses PostgreSQL to store player data as well as inventory and quest data. A backup Xml file is used to store the same information incase of a database error. When the database returns null, the backup XML file will be used to restore player data.

The game itself is built in C# using winform and applies OOP principles.

## TODO

All of my TODO is kept on 1Notes.txt. It's not pretty but it's how I'm doing it.
