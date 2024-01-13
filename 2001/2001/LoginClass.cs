namespace _2001
{
    public class UserLogin
    {

        //creates variabless that can be used across controllers
        public static bool Loggedin { get; set; } 
        public static int ProfileId { get; set; }
        public static bool Admin { get; set; }

        public UserLogin() //default class before logging in
        { 
            Loggedin = false;
            ProfileId = 0;
            Admin = false;
        }
    }
}
