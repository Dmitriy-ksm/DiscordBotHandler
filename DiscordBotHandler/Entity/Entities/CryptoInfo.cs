using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBotHandler.Entity.Entities
{
    public class CryptoInfo
    {
        [Key]
        public int Id { get; set; }
        public double EthUsd { get; set; }
        public int GasAvarage { get; set; }
        public string EthUsdTime { get; set; }
        public double EthBtc { get; set; }
        public string EthBtcTime { get; set; }
    }
}
