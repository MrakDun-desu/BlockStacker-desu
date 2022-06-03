﻿using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Spins;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    public class GameRecorder : MonoBehaviour
    {
        [SerializeField] private MediatorSO _mediator;

        [SerializeReference] public List<Message> ActionList = new();

        private void Awake()
        {
            _mediator.Register<InputActionMessage>(AddMessageToList);
            _mediator.Register<SpinSuccessfullMessage>(AddMessageToList);
        }

        private void AddMessageToList(Message message)
        {
            ActionList.Add(message);
            ActionList.Sort((a, b) => a.Time > b.Time ? 1 : -1);
        }

        public void ClearActionList()
        {
            ActionList.Clear();
        }
    }
}