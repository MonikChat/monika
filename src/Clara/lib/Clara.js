/**
 * @file Class file for Clara, extending the Eris client.
 * @author Ovyerus
 */

const Eris = require('eris');
const got = require('got');
const fs = require('fs');
const Redite = require('redite');
const {CommandHolder} = require(`${__dirname}/modules/CommandHolder`);
const LocaleManager = require(`${__dirname}/modules/LocaleManager`);
const Lookups = require(`${__dirname}/modules/Lookups`);
const path = require('path');

/**
 * Main class for Clara.
 * 
 * @prop {String[]} admins Array of people who have owner-like permissions over the bot.
 * @prop {Boolean} allowCommandUse If commands can be run.
 * @prop {String[]} blacklist Array of people who cannot use the bot.
 * @prop {CommandHolder} commands Command holder object.
 * @prop {String[]} commandFolders todo
 * @prop {ClaraConfig} config Configuration passed during construction.
 * @prop {Redite} db Database connection manager.
 * @prop {Boolean} loadCommands If the bot should load commands or not.
 * @prop {String[]} prefixes Array of all the prefixes that are able to be used by the bot.
 * @prop {Object} settings Settings cache for users and guilds.
 */
class Clara extends Eris.Client {
    /**
     * Creates a new Clara instance.
     * 
     * @param {ClaraConfig} config Configuration settings
     * @param {Object} options Eris client options.
     * @see https://abal.moe/Eris/docs/Client
     */
    constructor(config, options = {}) {
        if (!config && typeof config !== 'object') throw new TypeError('config is not an object.');
        
        super(config.token, options);
        if (!fs.existsSync(path.resolve(`${__dirname}`, '../', './data'))) fs.mkdirSync(path.resolve(`${__dirname}`, '../', './data'));
        if (!fs.existsSync(path.resolve(`${__dirname}`, '../', './data/data.json'))) fs.writeFileSync(path.resolve(`${__dirname}`, '../', './data/data.json'), '{"admins": [], "blacklist": []}');
        if (!fs.existsSync(path.resolve(`${__dirname}`, '../', './data/prefixes.json'))) fs.writeFileSync(path.resolve(`${__dirname}`, '../', './data/prefixes.json'), '[]');
        
        let tmp = JSON.parse(fs.readFileSync(path.resolve(`${__dirname}`, '../', './data/data.json')));

        this.blacklist = tmp.blacklist;
        this.admins = tmp.admins;
        this.config = config;
        this.prefixes = JSON.parse(fs.readFileSync(path.resolve(`${__dirname}`, '../', './data/prefixes.json'))).concat([config.mainPrefix]);

        this.lookups = new Lookups(this);
        this.localeManager = new LocaleManager();
        this.commands = new CommandHolder(this);
        this.db = new Redite({url: config.redisURL || config.redisUrl || 'redis://127.0.0.1/0'});

        this.loadCommands = true;
        this.allowCommandUse = false;
    }

    /**
    * Wait for a message in the specified channel from the specified user.
    *
    * @param {String} channelID ID of channel to wait in.
    * @param {String} userID ID of user to wait for.
    * @param {Function} [filter] Filter to pass to message. Must return true.
    * @param {Number} [timeout=15000] Time in milliseconds to wait for message.
    * @returns {Promise<Eris.Message>} Awaited message.
    */
    awaitMessage(channelID, userID, filter = () => true, timeout = 15000) {
        return new Promise((resolve, reject) => {
            if (!channelID || typeof channelID !== 'string') {
                reject(new TypeError('channelID is not string.'));
            } else if (!userID || typeof userID !== 'string') {
                reject(new TypeError('userId is not string.'));
            } else {
                var responded, rmvTimeout;

                var onCrt = msg => {
                    if (msg.channel.id === channelID && msg.author.id === userID && filter(msg)) responded = true;

                    if (responded) {
                        this.removeListener('messageCreate', onCrt);
                        clearInterval(rmvTimeout);
                        resolve(msg);
                    }
                };

                this.on('messageCreate', onCrt);

                rmvTimeout = setTimeout(() => {
                    if (!responded) {
                        this.removeListener('messageCreate', onCrt);
                        reject(new Error('Message await expired.'));
                    }
                }, timeout);
            }
        });
    }

    /**
     * POSTs guild count to various bot sites.
     */
    async postGuildCount() {
        if (this.config.discordBotsPWKey) {
            try {
                await got(`https://bots.discord.pw/api/bots/${this.user.id}/stats`, {
                    method: 'POST',
                    headers: {
                        Authorization: this.config.discordBotsPWKey,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        server_count: this.guilds.size
                    })
                });
            } catch(err) {
                logger.error(`Error POSTing to DBots:\n${err}`);
                return;
            }

            logger.info('Posted to DBots.');
        }

        if (this.config.discordBotsOrgKey) {
            try {
                await got(`https://discordbots.org/api/bots/${this.user.id}/stats`, {
                    method: 'POST',
                    headers: {
                        Authorization: this.config.discordBotsOrgKey,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        server_count: this.guilds.size
                    })
                });
            } catch(err) {
                logger.error(`Error POSTing to discordbots.org:\n${err}`);
                return;
            }

            logger.info('POSTed to discordbots.org.');
        }
    }

    /**
     * POST something to Hastebin.
     * 
     * @param {String} str Content to POST.
     * @returns {Promise<String>} Returned key.
     */
    async hastePost(str) {
        if (typeof str !== 'string') throw new TypeError('str is not a string');

        let res = await got('https://hastebin.com/documents', {
            method: 'POST',
            body: str
        });

        return JSON.parse(res.body).key;
    }

    /**
     * Check if a user has elevated bot permissions.
     * 
     * @param {String} userID ID of the user to check.
     * @returns {Boolean} If the user has perms.
     */
    checkBotPerms(userID) {
        return userID === this.config.ownerID || this.admins.includes(userID);
    }

    /**
     * Check if a user is blacklisted.
     * 
     * @param {String} userID ID of the user to check.
     * @returns {Boolean} If the user is blacklisted.
     */
    isBlacklisted(userID) {
        return this.blacklist.includes(userID);
    }

    async reloadData() {
        let res = await new Promise((resolve, reject) => fs.readFile('../data/data.json', (err, r) => {
            if (err) reject(err);
            else resolve(JSON.parse(r));
        }));

        this.admins = res.admins;
        this.blacklist = res.blacklist;
    }

    /**
    * Initialize settings for a guild.
    *
    * @param {String} guildID ID of guild to init settings for.
    * @returns {Object} Settings for the guild.
    */
    async initGuildSettings(guildID) {
        if (typeof guildID !== 'string') throw new TypeError('guildID is not a string.');
        if (await this.db.has(guildID)) return await this.db.guildID.get;

        let settings = {
            id: guildID,
            locale: 'en-UK',
            greeting: {
                enabled: false,
                channelID: null,
                message: null
            },
            goodbye: {
                enabled: false,
                channelID: null,
                message: null
            },
            ranks: {
                limit: 0,
                roles: []
            }
        };
        
        await this.db[guildID].set(settings);
        return settings;
    }

    /**
    * Get the settings for a guild.
    *
    * @param {String} guildID ID of guild to get settings for
    * @returns {Object} Settings for the guild.
    */
    async getGuildSettings(guildID) {
        if (typeof guildID !== 'string') throw new TypeError('guildID is not a string.');
        if (!await this.db.has(guildID)) return await this.initGuildSettings(guildID);

        return await this.db[guildID].get;
    }

    /**
    * Initialize settings for a user.
    *
    * @param {String} userID ID of user to init settings for.
    * @returns {Object} Settings for the user.
    */
    async initUserSettings(userID) {
        if (typeof userID !== 'string') throw new TypeError('userID is not a string.');
        if (await this.db.has(userID)) return await this.db[userID].get;

        let settings = {
            id: userID,
            locale: 'en-UK',
            partner: null
        };

        await this.db[userID].set(settings);
        return settings;
    }

    /**
    * Get the settings for a user.
    *
    * @param {String} userID ID of user to get settings for.
    * @returns {Object} Settings for the user.
    */
    async getUserSettings(userID) {
        if (typeof userID !== 'string') throw new TypeError('userID is not a string.');
        if (!await this.db.has(userID)) return await this.initUserSettings(userID);
        
        return await this.db[userID].get;
    }

    /**
     * Check if the bot has a permission in a channel.
     * 
     * @param {String} permission The permission to check.
     * @param {Eris.Channel} channel The channel to check.
     * @returns {Boolean} If the user has the permission.
     */
    hasPermission(permission, channel) {
        // Check if permission actually exists.
        if (!Object.keys(Eris.Constants.Permissions).includes(permission)) return false;

        return channel.permissionsOf(this.user.id).has(permission);
    }

    get commandFolders() {
        let cmdDirs = fs.readdirSync(this.commandsDir).map(d => ({[d]: fs.readdirSync(`${this.commandsDir}/${d}`)}));
        let allCmds = {};
    
        // Go from an array of objects to an object of arrays.
        cmdDirs.forEach(d => Object.assign(allCmds, d));
        cmdDirs = cmdDirs.map(e => Object.keys(e)[0]);
    
        // Turn folder names into proper paths for future ease (also make sure we only get folders).
        allCmds = Object.entries(allCmds).map(x => x[1].filter(y => fs.statSync(`${this.commandsDir}/${x[0]}/${y}`).isDirectory()));
        allCmds = allCmds.map((v, i) => v.map(x => `${this.commandsDir}/${cmdDirs[i]}/${x}`));
        allCmds = [].concat.apply([], allCmds);

        return allCmds;
    }
}

/**
 * Configuration used for Clara instances.
 * @see config.json.example
 * 
 * @prop {Boolean} [debug=false] Whether to output error stacks to console.
 * @prop {String} discordBotsOrgKey API key to use for posting stats to discordbots.org.
 * @prop {String} discordBotsPWKey API key to use for posting stats to bots.discord.pw.
 * @prop {String} gameName Text to use for playing status.
 * @prop {String} gameURL Stream url for playing status. Must be a Twitch link.
 * @prop {String} ibKey API key to use for ibsear.ch.
 * @prop {String} mainPrefix Default prefix for the bot.
 * @prop {Number} maxShards Maximum number of shards for the bot to use.
 * @prop {String} nasaKey API key for the NASAS commands.
 * @prop {String} osuApiKey API key to use for osu! commands.
 * @prop {String} ownerID ID of the person who has the most permissions for the bot.
 * @prop {Boolean} promiseWarnings Whether to get the shitty errors from the Promise library.
 * @prop {Object} redisUrl `redis://` url to connect to. Defaults to `redis://127.0.0.1/0`
 * @prop {String} sauceKey API key to use for SauceNAO.
 * @prop {String} token Token to use when connecting to Discord.
 * @prop {String} twitchKey Key to use for Twitch.
 * @prop {String} ytSearchKey Key to use for searching YouTube tracks.
 */
class ClaraConfig { // eslint-disable-line
    // Only here for documentation purposes.
}

module.exports = Clara;