token = "token_here"
prefix = "s)"
local pref_table =
{
    ["ping"] = "pong!"
}
local no_pref_table =
{
    ["que"] = "so",
    ["rra"] = "tu vieja",
    ["como"] = "verga",
}

function string.starts(String, Start)
    return string.sub(String, 1, string.len(Start)) == Start
end

function onMessage(message)
    if string.starts(message.Content, prefix) then
        local args = message.Content.sub(message.Content, string.len(prefix) + 1)

        local shit = pref_table[args]
        if shit == nil then
            print("No command matching " .. args)
            return 0
        end

        if type(shit) == "function" then
            shit()
        elseif type(shit) == "string" then
            message.Channel:SendMessageAsync(shit)
        end
    else
        local shit2 = no_pref_table[message.Content]
        if shit2 == nil then
            print("No command matching " .. message.Content)
            return 0
        end

        message.Channel:SendMessageAsync(shit2)
    end
end
