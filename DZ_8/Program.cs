using System;
using System.Collections.Generic;

namespace Homework9_1
{
    class Song
    {
        private string name;
        private string author;
        public Song prev;

        public Song()
        {
            name = "Неизвестно";
            author = "Неизвестно";
            prev = null;
        }
        public Song(string name, string author)
        {
            this.name = name;
            this.author = author;
            this.prev = null;
        }
        public Song(string name, string author, Song prevSong)
        {
            this.name = name;
            this.author = author;
            this.prev = prevSong;
        }

        public void SetName(string name) { this.name = name; }
        public void SetAuthor(string author) { this.author = author; }
        public void SetPrev(Song prev) { this.prev = prev; }

        public void PrintInfo()
        {
            Console.WriteLine($"Песня: '{name}', Исполнитель: {author}");
        }

        public string Title()
        {
            return $"{name} - {author}";
        }

        public override bool Equals(object d)
        {
            if (d == null) return false;
            if (d is Song otherSong)
            {
                return this.name == otherSong.name && this.author == otherSong.author;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, author);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Song mySong = new Song();
            mySong.PrintInfo();

            Song song1 = new Song("Династия", "madk1d");
            song1.PrintInfo();

            Song song2 = new Song("Морем", "АлоэВер", song1);
            song2.PrintInfo();

            Console.WriteLine($"Предыдущая песня для song2: {song2.prev.Title()}");
        }
    }
}