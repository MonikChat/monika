"""
DDLC-style poem generator written in Python, replacing the original Lua one by FiniteReality.
Created by Ovyerus (https://github.com/Ovyerus) and licensed under the MIT License.
"""
import json
import textwrap

from io import BytesIO
from aiohttp import web
from PIL import Image, ImageDraw, ImageFont

"""
API Overview
------------

There are two accepted parameters, `poem` and `font`.
`poem` is the only required parameter, and specifies the content of the poem to generate.
`font` is optional, and if it is not a supported font, it will default to DEFAULT_FONT (usually `m1`).
Parameters can either be sent by a JSON body, or by a query string (?poem=Hello%20world&font=y1)

Requests can either be a GET or a POST, with the former being allowed to support browsers.
Both methods of submitting data are supported with both methods, however most things that send a GET will only allow the query string method (as far as I am aware).
"""

WIDTH = 720
HEIGHT = 500
PADDING = 100 # px
DEFAULT_FONT = 'm1'
DEFAULT_BG = Image.open('./backgrounds/poem.jpg')

# Cache the fonts and backgrounds.

FONTS = {
    'm1': ImageFont.truetype('./fonts/m1.ttf', 34), # Monika
    's1': ImageFont.truetype('./fonts/s1.ttf', 34), # Sayori
    'n1': ImageFont.truetype('./fonts/n1.ttf', 28), # Natsuki
    'y1': ImageFont.truetype('./fonts/y1.ttf', 32), # Yuri (Normal)
    'y2': ImageFont.truetype('./fonts/y2.ttf', 40), # Yuri (Fast)
    'y3': ImageFont.truetype('./fonts/y3.ttf', 18) # Yuri (Obsessed)
}

BACKGROUNDS = {
    'y2': Image.open('./backgrounds/poem_y1.jpg'),
    'y3': Image.open('./backgrounds/poem_y2.jpg')
}

def break_text(text, font, max_width):
    word_list = text.split(' ')
    tmp = ''
    wrapped = []

    # Iterates through all the words, and if the width of a tempory variable and the word is larger than the max width,
    # the string is appended to a list, and the string is set to the word plus a space, otherwise the word is just added
    # to the temporary string with a space.
    for word in word_list:
        if font.getsize(tmp + word)[0] > max_width:
            wrapped.append(tmp.strip())
            tmp = word + ' '
        else:
            tmp += word + ' '

    # Add remaining words
    if tmp:
        wrapped.append(tmp.strip())

    return '\n'.join(wrapped)

async def handle_request(req):
    body = await req.text()
    is_json = True

    if not body:
        body = req.query
        is_json = False

    if not body :
        return web.Response(status=400, text='{"error": "No body or query string.", "code": 0}', content_type='application/json')

    if is_json:
        body = json.loads(body)

    if 'poem' not in body:
        return web.Response(status=400, text='{"error": "Missing required field: `poem`.", "code": 1}', content_type='application/json')

    if type(body['poem']) is not str:
        return web.Response(status=400, text='{"error": "Field `poem` is not a string.", "code": 2}', content_type='application/json')

    if not body['poem']:
        return web.Response(status=400, text='{"error": "Field `poem` is empty.", "code": 3}', content_type='application/json')

    poem = body['poem'].replace('\r', '').replace('\n', '\u2426')

    _font = body.get('font', DEFAULT_FONT)

    if _font not in FONTS:
        _font = DEFAULT_FONT

    bg = BACKGROUNDS.get(_font, DEFAULT_BG).copy()
    font = FONTS[_font]

    draw = ImageDraw.Draw(bg)

    b = BytesIO()
    poem = break_text(poem, font, bg.width - PADDING * 2).replace('\u2426', '\n\n')
    height = max(bg.height, draw.textsize(poem, font)[1] + PADDING * 2)

    if height > bg.height:
        bg = bg.resize((bg.width, height), Image.BICUBIC)

    draw = ImageDraw.Draw(bg)

    draw.text((PADDING , PADDING), poem, '#000000', font)
    bg.save(b, 'png')
    b.seek(0)

    return web.Response(body=b, content_type='image/png')

app = web.Application()

app.router.add_post('/generate', handle_request)
app.router.add_get('/generate', handle_request)

print('Loading poem server')
web.run_app(app)