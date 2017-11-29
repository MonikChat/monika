/**
 * @file Ecosystem file for PM2.
 * @description: Sysadmins: this is what you will execute using PM2.
 */
module.exports = {
    apps: [
        {
            name: 'Monika.Gateway',
            script: './Clara/bot.js',
            error_file: '../logs/Gateway/err.log',
            out_file: '../logs/Gateway/out.log',
            log_date_format: 'YYYY-MM-DD HH:mm Z',
            watch: ['./Clara/bot.js'],
            ignore_watch: ['data', 'logs', 'cmd', 'cache', 'node_modules'],
            args: ['--color']
        },
        {
            name: 'Monika.PoemService',
            interpreter: '/user/bin/python3.6',
            script: './ImageGenerator/main.py',
            error_file: '../logs/PoemService/err.log',
            out_file: '../logs/PoemService/out.log',
            log_date_format: 'YYYY-MM-DD HH:mm Z',
            ignore_watch: ['data', 'logs', 'cmd', 'cache', 'node_modules'],
            args: ['--color']
        }
    ]
};