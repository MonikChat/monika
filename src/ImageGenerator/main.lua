local httpd = require("xavante.httpd")
local xavante = require("xavante")
local gd = require("gd")
local json = require("cjson")

local WIDTH = 720
local HEIGHT = 500
local PADDING = 100 -- px

local FREETYPE_CFG = {
    ["m1"] = { -- monika
        fontsize = 34
    },

    ["s1"] = { -- sayori
        fontsize = 34
    },

    ["n1"] = { -- natsuki
        fontsize = 28
    },

    ["y1"] = { -- yuri 1 (normal)
        fontsize = 32
    },
    ["y2"] = { -- yuri 2 (fast)
        fontsize = 40
    },
    ["y3"] = { -- yuri 3 (obsessed)
        fontsize = 18,
        disable_kerning = true
    }
}

local BACKGROUNDS = {
    ["m1"] = nil,
    ["n1"] = nil,
    ["s1"] = nil,
    ["y1"] = nil,
    ["y2"] = "poem_y1.jpg",
    ["y3"] = "poem_y2.jpg"
}

local dummy = gd.createTrueColor(WIDTH, HEIGHT)
local dummyColor = dummy:colorAllocate(255, 255, 255)

-- Word wrapping from http://lua-users.org/wiki/TextProcessing
-- Slightly modified to remove unnecessary parameters in this context
local strrep, strsub = string.rep, string.sub
function wrap(s, w)
    w = w or 78
    local lstart, len = 1, #s
    while len - lstart > w do
        local i = lstart + w
        while i > lstart and strsub(s, i, i) ~= " " do i = i - 1 end
        local j = i
        while j > lstart and strsub(s, j, j) == " " do j = j - 1 end
        s = strsub(s, 1, j) .. "\n" .. strsub(s, i + 1, -1)
        local change = 1 - (i - j)
        lstart = j + change
        len = len + change
    end
    return s
end

local function handleRequest(req, res)
    if req.cmd_mth ~= "POST" then
        return httpd.err_405(req, res)
    end

    local body = req.socket:receive(
        req.headers["content-length"])

    body = body and json.decode(body)

    if not body then
        res.statusline = "HTTP/1.1 400 Bad Request"
        res.headers["Content-Type"] = "application/json"
        res.content = [[{"error": "missing body"}]]

        return res
    end

    local rawPoem = body.poem:gsub("\r","")
    local poem = {}
    for line, terminator in rawPoem:gmatch("([^\n]*)(\n?)") do
        if #line > 0 then
            poem[#poem+1] = wrap(line, 75)
        elseif #terminator > 0 then
            poem[#poem+1] = ""
        end
    end
    body.poem = table.concat(poem, "\n")

    local background = BACKGROUNDS[body.font] or "poem.jpg"
    local bkg = gd.createFromJpeg("backgrounds/"..background)

    local fontFile = "./fonts/" .. body.font .. ".ttf"
    local fontSize = FREETYPE_CFG[body.font] and
        FREETYPE_CFG[body.font].fontsize or 14

    local llx, lly, lrx, lry, urx, ury, ulx, uly, font =
        dummy:stringFTEx(dummyColor, fontFile,
        fontSize, 0, 0, 0,
        body.poem, FREETYPE_CFG[body.font] or {})

    local width = math.max(150, math.abs(urx - ulx))
        + 2 * PADDING
    local height = math.max(150, math.abs(lly - uly))
        + 2 * PADDING

    local img = gd.createTrueColor(width, height)
    img:copyResampled(bkg, 0, 0, 0, 0, width, height,
        bkg:sizeXY())
    local black = img:colorAllocate(0, 0, 0)
    img:stringFTEx(black, fontFile,
        fontSize, 0, PADDING,
        PADDING + (uly < 0 and -uly or 0),
        body.poem, FREETYPE_CFG[body.font] or {})

    res.statusline = "HTTP/1.1 200 OK"
    res.headers["Content-Type"] = "image/png"
    res.content = img:pngStr()

    return res
end

xavante.HTTP{
    server = {host = "localhost", port = 8080},

    defaultHost = {
        rules = {
            {
                match = "/generate",
                with = function(req, res)
                    local succ, _res = pcall(function() handleRequest(req, res) end)
                    if succ then
                        return _res
                    else
                        res.statusline = "HTTP/1.1 500 Internal Server Error"
                        res.headers["Content-Type"] = "text/plain"
                        res.content = _res
                    end
                end
            }
        }
    }
}

xavante.start()