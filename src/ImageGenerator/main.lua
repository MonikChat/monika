local httpd = require("xavante.httpd")
local xavante = require("xavante")
local gd = require("gd")
local json = require("cjson")

local WIDTH = 720
local HEIGHT = 500

local dummy = gd.createTrueColor(WIDTH, HEIGHT)
local dummyColor = dummy:colorAllocate(255, 255, 255)

local extra = {
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

local backgrounds = {
    ["m1"] = nil,
    ["n1"] = nil,
    ["s1"] = nil,
    ["y1"] = nil,
    ["y2"] = "poem_y1.jpg",
    ["y3"] = "poem_y2.jpg"
}

local padding = 100 -- px

local wrapping = ("%S*%s?"):rep(16)

xavante.HTTP{
    server = {host = "localhost", port = 8080},

    defaultHost = {
        rules = {
            {
                match = "/generate",
                with = function(req, res)
                    if req.cmd_mth ~= "POST" then
                        return httpd.err_405(req, res)
                    end

                    local body = req.socket:receive(req.headers["content-length"])

                    body = body and json.decode(body)

                    if not body then
                        res.statusline = "HTTP/1.1 400 Bad Request"
                        res.headers["Content-Type"] = "application/json"
                        res.content = [[{"error": "missing body"}]]

                        return res
                    end

                    body.poem = body.poem:gsub('\r', '')

                    local poem = {}
                    for line, terminator in body.poem:gmatch("([^\n]*)(\n?)") do
                        for wrap in line:gmatch(wrapping) do
                            if #wrap > 0 then
                                poem[#poem+1] = wrap
                                    :gsub('&', '&amp;')
                                    :gsub("^%s*(.-)%s*$", "%1")
                            end
                        end
                        if #line == 0 and #terminator == 1 then
                            poem[#poem+1] = ''
                        end
                    end
                    body.poem = table.concat(poem, '\n')

                    local background = backgrounds[body.font] or "poem.jpg"
                    local bkg = gd.createFromJpeg("backgrounds/"..background)

                    local fontFile = "./fonts/" .. body.font .. ".ttf"
                    local fontSize = extra[body.font] and extra[body.font].fontsize or 14

                    local llx, lly, lrx, lry, urx, ury, ulx, uly, font =
                        dummy:stringFTEx(dummyColor, fontFile,
                        fontSize, 0, 0, 0,
                        body.poem, extra[body.font] or {})

                    local width = math.max(150, math.abs(urx - ulx)) + 2 * padding
                    local height = math.max(150, math.abs(lly - uly)) + 2 * padding

                    local img = gd.createTrueColor(width, height)
                    img:copyResampled(bkg, 0, 0, 0, 0, width, height, bkg:sizeXY())
                    local black = img:colorAllocate(0, 0, 0)
                    img:stringFTEx(black, fontFile,
                        fontSize, 0, padding, padding + (uly < 0 and -uly or 0),
                        body.poem, extra[body.font] or {})

                    res.statusline = "HTTP/1.1 200 OK"
                    res.headers["Content-Type"] = "image/png"
                    res.content = img:pngStr()

                    --img:png("tmp.png")

                    return res
                end
            }
        }
    }
}

xavante.start()