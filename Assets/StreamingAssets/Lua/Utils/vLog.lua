--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
function vLog(param)
    local str = dumptree(param);
    Debug.Log("<color=orange>" .. str .. "</color>");
end

function vLogWarning(param)
    local str = dumptree(param);
    Debug.LogWarning("<color=orange>" .. str .. "</color>");
end

function vLogError(param)
    local str = dumptree(param);
    CS.UnityEngine.Debug.LogError("<color=orange>" .. str .. "</color>");
end

function dumptree(obj, width)
	local cache_table = {};
    local dump_obj;
    local end_flag = {};

    local function make_indent(layer, is_end)
        local subIndent = string.rep("  ", width)
        local indent = "";
        end_flag[layer] = is_end;
        local subIndent = string.rep("  ", width)
        for index = 1, layer - 1 do
            if end_flag[index] then
                indent = indent.." "..subIndent
            else
                indent = indent.."|"..subIndent
            end
        end

        if is_end then
            return indent.."└"..string.rep("─", width).." "
        else
            return indent.."├"..string.rep("─", width).." "
        end
    end

    local function make_quote(str)
        str = string.gsub(str, "[%c\\\"]", {
            ["\t"] = "\\t",
            ["\r"] = "\\r",
            ["\n"] = "\\n",
            ["\""] = "\\\"",
            ["\\"] = "\\\\",
        })
        return "\""..str.."\""
    end

    local function dump_key(key)
        if type(key) == "number" then
            return key .. "] "
        elseif type(key) == "string" then
            return tostring(key).. ": "
        end
    end

    local function dump_val(val, layer)
        if type(val) == "table" then
			if cache_table[val] == nil then
				cache_table[val] = true;
				return dump_obj(val, layer)
			else
				return tostring(val);
			end
        elseif type(val) == "string" then
            return make_quote(val)
        else
            return tostring(val)
        end
        return tostring(val)
    end

    local function count_elements(obj)
        local count = 0
        for k, v in pairs(obj) do
            count = count + 1
        end
        return count
    end

    dump_obj = function(obj, layer)
        if type(obj) ~= "table" then
            return count_elements(obj)
        end

        layer = layer + 1
        local tokens = {}
        local max_count = count_elements(obj)
        local cur_count = 1
        for k, v in pairs(obj) do
            local key_name = dump_key(k)
			if string.find(key_name,"__") ~= 1 then
				if type(v) == "table" then
					if cache_table[v] == nil then
						key_name = key_name.."\n"
					else
						key_name = key_name
					end
				end
				if type(v) ~= "function" then
					table.insert(tokens, make_indent(layer, cur_count == max_count)
						.. key_name .. dump_val(v, layer))
				end
				cur_count = cur_count + 1
			end
        end

        if max_count == 0 or #tokens == 0 then
            table.insert(tokens, make_indent(layer, true) .. "{ }")
        end

        return table.concat(tokens, "\n")
    end

	if type(obj) ~= "table" then
        return "the params you input is "..type(obj)..
        ", not a table, the value is --> "..tostring(obj)
    end

    width = width or 2
    return "root-->"..tostring(obj).."\n"..dump_obj(obj, 0)
end
--endregion
