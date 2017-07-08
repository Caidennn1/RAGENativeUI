﻿namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Rendering;
    using RAGENativeUI.Utility;

    public class Menu
    {
        public PointF Location { get; set; } = new PointF(480, 17);

        private MenuItemsCollection items;
        public MenuItemsCollection Items { get { return items; } set { items = value ?? throw new InvalidOperationException($"The menu {nameof(Items)} can't be null."); } }

        private MenuSkin skin;
        public MenuSkin Skin { get { return skin; } set { skin = value ?? throw new InvalidOperationException($"The menu {nameof(Skin)} can't be null."); } }

        private MenuBanner banner;
        public MenuBanner Banner { get { return banner; } set { banner = value ?? throw new InvalidOperationException($"The menu {nameof(Banner)} can't be null."); } }

        private MenuSubtitle subtitle;
        public MenuSubtitle Subtitle { get { return subtitle; } set { subtitle = value ?? throw new InvalidOperationException($"The menu {nameof(Subtitle)} can't be null."); } }

        private int selectedIndex;
        public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = MathHelper.Clamp(value, 0, Items.Count); } }
        public MenuItem SelectedItem { get { return Items[selectedIndex]; } set { selectedIndex = Items.IndexOf(value); } }

        private MenuControls controls;
        public MenuControls Controls { get { return controls; } set { controls = value ?? throw new InvalidOperationException($"The menu {nameof(Controls)} can't be null."); } }

        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                if (value == width)
                    return;
                width = value;
                Banner.Size = new SizeF(width, Banner.Size.Height);
                Subtitle.Size = new SizeF(width, Subtitle.Size.Height);
                for (int i = 0; i < Items.Count; i++)
                {
                    MenuItem item = Items[i];
                    item.Size = new SizeF(width, item.Size.Height);
                }
            }
        }

        public Menu(string title, string subtitle)
        {
            Items = new MenuItemsCollection(this);
            Skin = MenuSkin.DefaultSkin;
            Banner = new MenuBanner();
            Subtitle = new MenuSubtitle();
            Controls = new MenuControls();

            Banner.Title = title;
            Subtitle.Text = subtitle;

            Width = 430.0f;
        }

        public virtual void ProcessInput()
        {
            if (Game.IsKeyDown(System.Windows.Forms.Keys.Up))
            {
                MoveUp();
            }
            else if (Game.IsKeyDown(System.Windows.Forms.Keys.Down))
            {
                MoveDown();
            }
        }

        public void MoveUp()
        {
            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = Items.Count - 1;

            SelectedIndex = newIndex;
        }

        public void MoveDown()
        {
            int newIndex = SelectedIndex + 1;

            if (newIndex > (Items.Count - 1))
                newIndex = 0;

            SelectedIndex = newIndex;
        }

        public virtual void Draw(Graphics graphics)
        {
            float x = Location.X, y = Location.Y;

#if DEBUG
            bool debugDrawing = Game.IsKeyDownRightNow(System.Windows.Forms.Keys.D0);
            if (debugDrawing) Banner.DebugDraw(graphics, skin, x, y);
#endif
            Banner.Draw(graphics, skin, ref x, ref y);

#if DEBUG
            if (debugDrawing) Subtitle.DebugDraw(graphics, skin, x, y);
#endif
            Subtitle.Draw(graphics, skin, ref x, ref y);

            skin.DrawBackground(graphics, x, y - 1, Items.Max(m => m.Size.Width), Items.Sum(m => m.Size.Height));

            for (int i = 0; i < Items.Count; i++)
            {
                MenuItem item = Items[i];

#if DEBUG
                if (debugDrawing) item.DebugDraw(graphics, skin, i == SelectedIndex, x, y);
#endif
                item.Draw(graphics, skin, i == SelectedIndex, ref x, ref y);
            }
        }
    }


    public class MenuItemsCollection : Utility.BaseCollection<MenuItem>
    {
        protected internal Menu Menu { get; }

        public override MenuItem this[int index]
        {
            get { return base[index]; }
            set
            {
                base[index] = value;
                value.Size = new SizeF(Menu.Width, value.Size.Height);
            }

        }
        public MenuItemsCollection(Menu menu)
        {
            Menu = menu;
        }

        public override void Add(MenuItem item)
        {
            base.Add(item);
            item.Size = new SizeF(Menu.Width, item.Size.Height);
        }

        public override void Insert(int index, MenuItem item)
        {
            base.Insert(index, item);
            item.Size = new SizeF(Menu.Width, item.Size.Height);
        }
    }

    public class MenuControls
    {
        public Control Up { get; set; } = new Control(GameControl.FrontendUp);
        public Control Down { get; set; } = new Control(GameControl.FrontendDown);
        public Control Right { get; set; } = new Control(GameControl.FrontendRight);
        public Control Left { get; set; } = new Control(GameControl.FrontendLeft);
        public Control Accept { get; set; } = new Control(GameControl.FrontendAccept);
        public Control Cancel { get; set; } = new Control(GameControl.FrontendCancel);
    }
}
