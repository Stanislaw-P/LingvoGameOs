using System.Collections.Generic;
using LingvoGameOs.Models;

namespace LingvoGameOs.Repositories;

class PlayerRepository
{
    static List<Player> players = [];

    public Player GetById(int id)
    {
        Player? player = players.FirstOrDefault(x => x.Id == id);

        if (player == null)
            throw new Exception("User not found");

        return player;
    }
}
