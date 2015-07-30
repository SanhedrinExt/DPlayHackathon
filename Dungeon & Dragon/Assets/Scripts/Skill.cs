using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace SkillProvider
{
    public abstract class Skill : NetworkBehaviour
    {
        //time In milliseconds:
        [SerializeField]
        private readonly int k_Cooldown;
        public int CooldownTimer { get; private set; }
        private DateTime m_LastUsed = new DateTime();

        protected bool m_Active = true;

        public Skill(int i_Cooldown)
        {
            m_LastUsed = DateTime.Now;
            k_Cooldown = i_Cooldown;
            CooldownTimer = k_Cooldown / 2;
        }

        void Update()
        {
            if (CooldownTimer > 0)
            {
                TimeSpan deltaTime = DateTime.Now.Subtract(m_LastUsed);
                CooldownTimer -= deltaTime.Milliseconds;
            }
        }

        protected abstract void Activate();
        public void UseSkill()
        {
            if (m_Active && CooldownTimer <= 0)
            {
                Activate(); // injection
                StackableSkill self = this as StackableSkill;
                if (self != null)
                {
                    self.DeStack();
                }

                ResetCooldwon();
            }
        }

        private void ResetCooldwon()
        {
            m_LastUsed = DateTime.Now;
            CooldownTimer = k_Cooldown;
        }
    }

    public abstract class StackableSkill : Skill
    {
        [SerializeField]
        private readonly int k_MaxStacks;
        public int CurrStacks { get; private set; }

        public StackableSkill(int i_Cooldown, int i_MaxStacks) : base(i_Cooldown)
        {
            k_MaxStacks = i_MaxStacks;
            CurrStacks = k_MaxStacks;
        }

        public void DeStack()
        {
            if (CurrStacks != 0)
            {
                CurrStacks--;
                if (CurrStacks == 0)
                {
                    m_Active = false;
                }
            }
        }
    }
}