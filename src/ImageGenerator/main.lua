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

local wrapping = ("(\n*)(%s)(\n*)"):format(("[^\n]?"):rep(80))

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
                    local rawPoem = body.poem

                    local poem = { }
                    local start, finish, line = 1
                    while not finish or finish < #rawPoem do
                        start, finish, leadingNewlines, line, trailingNewlines = rawPoem:find(wrapping, finish)

                        local lastSpace = line:find("%s%S*$")
                        if lastSpace then
                            local wordStart, wordFinish = rawPoem:find("%S+", start + lastSpace)
                    
                            if wordFinish > finish then
                                finish = finish - (wordFinish - finish) + #leadingNewlines
                                line = rawPoem:sub(start + #leadingNewlines, finish)
                            end
                        end

                        local trimmedLine = line
                            :gsub("&", "&amp;")
                            :gsub("^%s*(%S-)%s*$", "%1")

                        poem[#poem+1] = trimmedLine
                        for i = 1, #trailingNewlines-1 do
                            poem[#poem+1] = ""
                        end
                    end
                    body.poem = table.concat(poem, '\n')
                    print(body.poem)

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