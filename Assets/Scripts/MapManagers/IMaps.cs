public interface IMaps
{
    void Init(); //Initlializes the map
    void AddNewPlayers();  //Adds new players during that map
    void PrintCharacters();  //Prints the initlial characters

    //Need function for add and remove characters later on
    
    void CheckClearCondition(); //After every action checks if clear condition has been met
    void CheckMainChars();  //After every action checks if main characters have died
}
