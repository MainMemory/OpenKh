﻿namespace OpenKh.Game.Menu
{
    public interface IMenu
    {
        bool IsClosed { get; }
        int SelectedOption { get; }

        void Open();
        void Close();
        void Push(IMenu subMenu);

        void Update(double deltaTime);
        void Draw();
    }
}
