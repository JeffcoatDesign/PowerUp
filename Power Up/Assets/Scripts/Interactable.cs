using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string _verb;
    [SerializeField] private float _interactionCooldown = 2f;
    [SerializeField] private InputActionReference _actionReference;
    public string verb { get { return _verb; } }

    private bool _isFocused = false;
    private bool _hasInteracted = false;
    private bool _actionPerformed = false;
    private void OnEnable()
    {
        PlayerController.OnSetFocus += OnSetFocus;
    }

    private void OnDisable()
    {
        PlayerController.OnSetFocus -= OnSetFocus;
    }
    private void Update()
    {
        if (!_hasInteracted)
        {
            _actionPerformed = _actionReference.action.ReadValue<float>() == 1;
            if (_isFocused && _actionPerformed)
            {
                StartCoroutine(ResetInteraction());
                Interact();
            }
        }
    }
    void OnSetFocus(Interactable focus)
    {
        if (focus == this)
            _isFocused = true;
        else
        {
            _isFocused = false;
            _hasInteracted = false;
        }
    }
    public virtual void Interact()
    {
        
        Debug.Log("Interacted with: " + gameObject.name);
    }
    private IEnumerator ResetInteraction()
    {
        _hasInteracted = true;
        yield return new WaitForSeconds(_interactionCooldown);
        _hasInteracted = false;
    }
}
