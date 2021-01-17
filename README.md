![Build](https://github.com/NiJeTi/CinemaKeeper/workflows/Build/badge.svg)

# CinemaKeeper
## Guess what? It's a Discord bot!

It can:
- Lock voice channels
- Cast voice channels

---

### Voice channel lock
1. Ya 'n ya boiz enter desired voice channel
2. One of you requests to lock the channel and bot sets user limit (`!lock` or `!lock <number>` for certain amount of users)
3. One of you requests to unlock the channel and bot removes user limit (`!unlock`)

---

### Cast voice channel

#### Variant A:
1. Enter desired voice channel
2. Type `!castv`
3. Such a very good boi-bot will cast every user in the channel

#### Variant B:
1. Type `!castv <name>` where *name* can be either a full name of a voice channel or just a part of it
2. Such a very good boi-bot will cast every user in the channel

#### Variant C (for nerds):
1. Go to Discord settings - Appearance
2. Scroll down and enable *Developer Mode*
3. Go back to chats list
4. Right click on the name of desired voice channel
5. Click *Copy ID*
6. Type `!castv <id>`
7. Such a very good boi-bot will cast every user in the channel
