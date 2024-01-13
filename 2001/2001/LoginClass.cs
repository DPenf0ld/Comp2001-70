namespace _2001
{
    public class UserLogin
    {

        //creates variabless that can be used across controllers
        public static bool Loggedin { get; set; } 
        public static bool Admin { get; set; }
        public static int userloginid { get; set; }

        public UserLogin() //default class before logging in
        { 
            Loggedin = false;
            userloginid = 0;
            Admin = false;
        }
    }
}
