/**
 * @file Poem Generator command
 * @author Capuccino
 * @author Ovyerus
 */

const PoemHandler = require('./PoemHandler');
let handler;

exports.commands = [
    'poem'
];

exports.init = bot => {
    handler = new PoemHandler(bot.config.sayoriHost);
};

exports.poem = {
    desc: 'Make a poem!',
    usage: '<yuri | yuri2 | yuri3 | natsuki | monika | sayori> <poem>',
    async main(bot, ctx) {
        if (!/^(y(uri)?[1-3]?|m(onika)?|n(atsuki)?|s(ayori)?)$/i.test(ctx.args[0])) return await ctx.createMessage('Hey, provide me which kind of writing style I should use. run "help poem" for the acceptable names.');
        if (!ctx.args[1]) return await ctx.createMessage('Give me a poem to write with.');

        await ctx.channel.sendTyping();

        let character = ctx.args[0].slice(0, 1).toLowerCase();

        if (character === 'y' && /[1-3]$/.test(ctx.args[0])) character += ctx.args[0].slice(-1);
        else character += '1';
        
        handler.generatePoemJson(ctx.cleanSuffix.split(' ').slice(1).join(' '), character).then(res => {
            console.log(res);
            ctx.createMessage({embed: {
                title: "Here's your poem!",
                image: {url: JSON.parse(res).url}
            }});
        });
    }
};