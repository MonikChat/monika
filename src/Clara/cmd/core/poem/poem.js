/**
 * @file Poem Generator command
 * @author Capuccino
 * @author Ovyerus
 */
exports.commands = [
    'poem'
];

exports.poem = {
    desc: 'Make a poem!',
    usage: '<yuri | yuri2 | yuri3 | natsuki | monika | sayori> <poem>',
    main(bot, ctx) {
        if (!/^(yuri[1-3]?|monika|natsuki|sayori)$i/.test(ctx.args[0])) {
            await ctx.createMessage('Hey, Provide me which kind of writing style I should use. run "help poem" for the acceptable names.');
        } else if (!ctx.args[2]) {
            await ctx.createMessage('Give me a poem to write with.');
        } else {
            //TODO: CC @ovyerus for the Python Image generator RPC
            return new Error('Not implemented yet.');
        }
    }
}