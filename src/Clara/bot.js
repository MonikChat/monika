/**
  * @file the main file the makes everything work
  * @author Capuccino
  * @author Ovyerus
  * @author FiniteReality
  * @copyright Copyright (c) 2017 Capuccino, Ovyerus and the repository contributors.
  * @license MIT
  */

// Imports
const Clara = require(`${__dirname}/lib/Clara`);
let config;

try {
    config = require(`${__dirname}/config.json`);
} catch(_) {
    config = {
        /** @see {Link} https://github.com/ClarityMoe/Clara/issues/133 */
       
        token: process.env.DISCORD_TOKEN,
        debug: process.env.DEBUG,
        promiseWarnings: process.env.ENABLE_PROMISE_WARNS || false,
        mainPrefix: process.env.DEFAULT_PREFIX,
        gameName: process.env.GAME_NAME,
        gameURL: process.env.GAME_URL,
        ownerID: process.env.BOT_OWNER_ID,
        maxShards: process.env.INSTANCES,
        sayoriHost: process.env.POEM_HOST
    };
}

// Globals
global.mainDir = __dirname;
global.utils = require(`${__dirname}/lib/modules/utils`);
global.logger = require(`${__dirname}/lib/modules/Logger`);
global.Promise = require('bluebird');
global.got = require('got');

//bot stuff
const bot = new Clara(config, {
    seedVoiceConnections: true,
    maxShards: config.maxShards || 1,
    latencyThreshold: 420000000,
    defaultImageFormat: 'webp',
    defaultImageSize: 512,
    disableEvents: {
        TYPING_START: true
    }
});

bot.commandsDir = `${__dirname}/cmd`;
bot.unloadedPath = `${__dirname}/data/unloadedCommands.json`;

//Promise configuration
Promise.config({
    warnings: {wForgottenReturn: config.promiseWarnings || false},
    longStackTraces: config.promiseWarnings || false
});

require(`${__dirname}/lib/events`)(bot);
bot.connect();

exports.bot = bot;