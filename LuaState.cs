using NLua;
using NLua.Exceptions;

namespace DBotLua
{
    class LuaState
    {
        public static Lua state = new Lua();

        public static Task<string> refresh(string luaPath)
        {
            state.LoadCLRPackage();
            try
            {
                state.DoFile(luaPath);
                return Task.FromResult<string>("Success");
            }
            catch(LuaScriptException ex)
            {
                return Task.FromResult<string>(ex.Message);
            }
        }

        public static void check(string varCheck)
        {
            bool isTokenCheck = (varCheck == "token");
            bool varExists = (state[varCheck] != null);

            if (!varExists)
            {
                Console.WriteLine($"Couldn't find {varCheck} in the LUA file");
                Environment.Exit(1);
            }

            if (isTokenCheck)
            {
                bool tokenLengthMatches = (isTokenCheck && varExists && state[varCheck].ToString()!.Length == 72);

                if (!tokenLengthMatches)
                {
                    Console.WriteLine("Token length doesn't match the required length (72)");
                    Environment.Exit(1);
                }
            }
        }

        public static Task execute(string funcName, params object[] args)
        {
            LuaFunction luaFunc = (LuaFunction)state[funcName];
            dynamic res = luaFunc.Call(args).First();
            Console.WriteLine($"{funcName} returned {res}");

            return Task.CompletedTask;
        }
    }
}