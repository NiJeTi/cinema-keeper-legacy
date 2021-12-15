![Build](https://github.com/NiJeTi/CinemaKeeper/workflows/Build/badge.svg)

# CinemaKeeper
## Guess what? It's a Discord bot!

It can:
- Lock voice channels
- Mention voice channels
- Store the most stunning quotes of ur homies 

---

### Voice channel lock
1. Ya 'n ya boiz enter desired voice channel
2. One of you requests to lock the channel and bot sets user limit (`!lock` or `!lock <number>` for certain amount of users)
3. One of you requests to unlock the channel and bot removes user limit (`!unlock`)

---

### Mention voice channel

#### Variant A:
1. Enter desired voice channel
2. Type `!cast`
3. Such a very good boi-bot will mention every user in the channel

#### Variant B:
1. Type `!cast <name>` where *name* can be either a full name of a voice channel or just a part of it
2. Such a very good boi-bot will mention every user in the channel

#### Variant C (for nerds):
1. Go to Discord settings - Appearance
2. Scroll down and enable *Developer Mode*
3. Go back to chats list
4. Right click on the name of desired voice channel
5. Click *Copy ID*
6. Type `!cast <id>`
7. Such a very good boi-bot will mention every user in the channel

---

### Quotes storage

### Add
1. Use `/quote` slash command
2. Select user
3. Enter something that they said
4. Send
5. Quote has been saved

### Get
1. Use `/quote` slash command
2. Select user
3. Send
4. All quotes of the user has been displayed
