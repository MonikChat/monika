# Core

this is the MaaS concept bot, at the moment, it's all barebones but we have at least two services done.

## Running

You will need Node 8.10.0 and above with pm2 installed globally.

Then copy `config.json.example` to `Clara` as `config.json` and provide necessary credentials listed there.

You have two ways to run the bot and the services:
- **PM2**

  simply run `pm2 src/monika.config.js` which would run both the poem service and the gateway service.

- **systemd**
 
  We provided a template for systemd in `systemd`. Edit it out and cp to `/etc/systemd/system`. Then you need to run `systemctl start Monika.Discord` (assuming you kept the filename intact prior to cp). 

  Protip : Run it as a user spawned service, not as a service with priviledges.

## Roadmap

- [x] Clara Discord Gateway
- [ ] Rebecca Dialogflow/Tensorflow AI
- [x] Poem Generator