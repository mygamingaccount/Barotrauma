﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Linq;
using Barotrauma.Extensions;
#if CLIENT
using Barotrauma.SpriteDeformations;
#endif

namespace Barotrauma
{
    class JointParams : RagdollSubParams
    {
        public JointParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll) { }

        private string name;
        [Serialize("", true), Editable]
        public override string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = GenerateName();
                }
                return name;
            }
            set
            {
                name = value;
            }
        }

        public override string GenerateName() => $"Joint {Limb1} - {Limb2}";

        [Serialize(-1, true), Editable]
        public int Limb1 { get; set; }

        [Serialize(-1, true), Editable]
        public int Limb2 { get; set; }

        /// <summary>
        /// Should be converted to sim units.
        /// </summary>
        [Serialize("1.0, 1.0", true), Editable]
        public Vector2 Limb1Anchor { get; set; }

        /// <summary>
        /// Should be converted to sim units.
        /// </summary>
        [Serialize("1.0, 1.0", true), Editable]
        public Vector2 Limb2Anchor { get; set; }

        [Serialize(true, true), Editable]
        public bool CanBeSevered { get; set; }

        [Serialize(true, true), Editable]
        public bool LimitEnabled { get; set; }

        /// <summary>
        /// In degrees.
        /// </summary>
        [Serialize(0f, true), Editable]
        public float UpperLimit { get; set; }

        /// <summary>
        /// In degrees.
        /// </summary>
        [Serialize(0f, true), Editable]
        public float LowerLimit { get; set; }

        [Serialize(0.25f, true), Editable]
        public float Stiffness { get; set; }
    }

    class LimbParams : RagdollSubParams
    {
        public LimbParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll)
        {
            var spriteElement = element.GetChildElement("sprite");
            if (spriteElement != null)
            {
                normalSpriteParams = new SpriteParams(spriteElement, ragdoll);
                SubParams.Add(normalSpriteParams);
            }
            var damagedSpriteElement = element.GetChildElement("damagedsprite");
            if (damagedSpriteElement != null)
            {
                damagedSpriteParams = new SpriteParams(damagedSpriteElement, ragdoll);
                // Hide the damaged sprite params in the editor for now.
                //SubParams.Add(damagedSpriteParams);
            }
            var deformSpriteElement = element.GetChildElement("deformablesprite");
            if (deformSpriteElement != null)
            {
                deformSpriteParams = new SpriteParams(deformSpriteElement, ragdoll)
                {
                    Deformation = new LimbDeformationParams(deformSpriteElement, ragdoll)
                };
                deformSpriteParams.SubParams.Add(deformSpriteParams.Deformation);
                SubParams.Add(deformSpriteParams);
            }
        }

        public readonly SpriteParams normalSpriteParams;
        public readonly SpriteParams damagedSpriteParams;
        public readonly SpriteParams deformSpriteParams;

        private string name;
        [Serialize("", true), Editable]
        public override string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = GenerateName();
                }
                return name;
            }
            set
            {
                name = value;
            }
        }

        public override string GenerateName() => $"Limb {ID}";

        /// <summary>
        /// Note that editing this in-game doesn't currently have any effect (unless the ragdoll is recreated). It should be visible, but readonly in the editor.
        /// </summary>
        [Serialize(-1, true), Editable]
        public int ID { get; set; }

        [Serialize(LimbType.None, true), Editable]
        public LimbType Type { get; set; }

        [Serialize(true, true), Editable]
        public bool Flip { get; set; }

        [Serialize(0, true), Editable]
        public int HealthIndex { get; set; }

        [Serialize(0f, true), Editable(ToolTip = "Higher values make AI characters prefer attacking this limb.")]
        public float AttackPriority { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 500)]
        public float SteerForce { get; set; }

        [Serialize("0, 0", true), Editable(ToolTip = "Only applicable if this limb is a foot. Determines the \"neutral position\" of the foot relative to a joint determined by the \"RefJoint\" parameter. For example, a value of {-100, 0} would mean that the foot is positioned on the floor, 100 units behind the reference joint.")]
        public Vector2 StepOffset { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 1000)]
        public float Radius { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 1000)]
        public float Height { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 1000)]
        public float Width { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 10000)]
        public float Mass { get; set; }

        [Serialize(10f, true), Editable(MinValueFloat = 0, MaxValueFloat = 100)]
        public float Density { get; set; }

        [Serialize("0, 0", true), Editable(ToolTip = "The position which is used to lead the IK chain to the IK goal. Only applicable if the limb is hand or foot.")]
        public Vector2 PullPos { get; set; }

        [Serialize(-1, true), Editable(ToolTip = "Only applicable if this limb is a foot. Determines which joint is used as the \"neutral x-position\" for the foot movement. For example in the case of a humanoid-shaped characters this would usually be the waist. The position can be offset using the StepOffset parameter.")]
        public int RefJoint { get; set; }

        [Serialize(false, true), Editable]
        public bool IgnoreCollisions { get; set; }

        [Serialize("", true), Editable]
        public string Notes { get; set; }

        // Non-editable ->
        [Serialize(0.3f, true)]
        public float Friction { get; set; }

        [Serialize(0.05f, true)]
        public float Restitution { get; set; }
    }

    class SpriteParams : RagdollSubParams
    {
        public SpriteParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll) { }

        [Serialize("0, 0, 0, 0", true), Editable]
        public Rectangle SourceRect { get; set; }

        [Serialize("0.5, 0.5", true), Editable(DecimalCount = 2, ToolTip = "Relative to the collider.")]
        public Vector2 Origin { get; set; }

        [Serialize(0f, true), Editable(DecimalCount = 3)]
        public float Depth { get; set; }

        [Serialize("", true)]
        public string Texture { get; set; }

        public LimbDeformationParams Deformation { get; set; }
    }

    class LimbDeformationParams : RagdollSubParams
    {
        public LimbDeformationParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll)
        {
#if CLIENT
            Deformations = new Dictionary<SpriteDeformationParams, XElement>();
            foreach (var deformationElement in element.GetChildElements("spritedeformation"))
            {
                string typeName = deformationElement.GetAttributeString("typename", null) ?? deformationElement.GetAttributeString("type", "");
                SpriteDeformationParams deformation = null;
                switch (typeName.ToLowerInvariant())
                {
                    case "inflate":
                        deformation = new InflateParams(deformationElement);
                        break;
                    case "custom":
                        deformation = new CustomDeformationParams(deformationElement);
                        break;
                    case "noise":
                        deformation = new NoiseDeformationParams(deformationElement);
                        break;
                    case "jointbend":
                    case "bendjoint":
                        deformation = new JointBendDeformationParams(deformationElement);
                        break;
                    case "reacttotriggerers":
                        deformation = new PositionalDeformationParams(deformationElement);
                        break;
                    default:
                        DebugConsole.ThrowError($"SpriteDeformationParams not implemented: '{typeName}'");
                        break;
                }
                if (deformation != null)
                {
                    deformation.TypeName = typeName;
                }
                Deformations.Add(deformation, deformationElement);
            }
#endif
        }

#if CLIENT
        public Dictionary<SpriteDeformationParams, XElement> Deformations { get; private set; }

        public override bool Deserialize(XElement element = null, bool recursive = true)
        {
            base.Deserialize(element, recursive);
            Deformations.ForEach(d => d.Key.SerializableProperties = SerializableProperty.DeserializeProperties(d.Key, d.Value));
            return SerializableProperties != null;
        }

        public override bool Serialize(XElement element = null, bool recursive = true)
        {
            base.Serialize(element, recursive);
            Deformations.ForEach(d => SerializableProperty.SerializeProperties(d.Key, d.Value));
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            Deformations.ForEach(d => d.Key.SerializableProperties = SerializableProperty.DeserializeProperties(d.Key, d.Value));
        }
#endif
    }

    class ColliderParams : RagdollSubParams
    {
        public ColliderParams(XElement element, RagdollParams ragdoll, string name = null) : base(element, ragdoll)
        {
            Name = name;
        }

        private string name;
        [Serialize("", true), Editable]
        public override string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = GenerateName();
                }
                return name;
            }
            set
            {
                name = value;
            }
        }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 1000)]
        public float Radius { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 1000)]
        public float Height { get; set; }

        [Serialize(0f, true), Editable(MinValueFloat = 0, MaxValueFloat = 1000)]
        public float Width { get; set; }
    }

    #region TODO
    class LimbAttackParams : RagdollSubParams
    {
        public LimbAttackParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll)
        {
        }

        public Attack Attack { get; set; }

        // new SerializableEntityEditor(ParamsEditor.Instance.EditorBox.Content.RectTransform, limb.attack, inGame: false, showName: true);
    }

    class DamageModifierParams : RagdollSubParams
    {
        public DamageModifierParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll)
        {
        }
    }

    class AfflictionParams : RagdollSubParams
    {
        public AfflictionParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll)
        {
        }
    }

    class StatusEffectParams : RagdollSubParams
    {
        public StatusEffectParams(XElement element, RagdollParams ragdoll) : base(element, ragdoll)
        {
        }
    }
    #endregion

    abstract class RagdollSubParams : ISerializableEntity
    {
        public virtual string Name { get; set; }
        public Dictionary<string, SerializableProperty> SerializableProperties { get; private set; }
        public XElement Element { get; set; }
        public XElement OriginalElement { get; protected set; }
        public List<RagdollSubParams> SubParams { get; set; } = new List<RagdollSubParams>();
        public RagdollParams Ragdoll { get; private set; }

        public virtual string GenerateName() => Element.Name.ToString();

        public RagdollSubParams(XElement element, RagdollParams ragdoll)
        {
            Element = element;
            OriginalElement = new XElement(element);
            Ragdoll = ragdoll;
            SerializableProperties = SerializableProperty.DeserializeProperties(this, element);
        }

        public virtual bool Deserialize(XElement element = null, bool recursive = true)
        {
            element = element ?? Element;
            SerializableProperties = SerializableProperty.DeserializeProperties(this, element);
            if (recursive)
            {
                SubParams.ForEach(sp => sp.Deserialize());
            }
            return SerializableProperties != null;
        }

        public virtual bool Serialize(XElement element = null, bool recursive = true)
        {
            element = element ?? Element;
            SerializableProperty.SerializeProperties(this, element, true);
            if (recursive)
            {
                SubParams.ForEach(sp => sp.Serialize());
            }
            return true;
        }

        public virtual void SetCurrentElementAsOriginalElement()
        {
            OriginalElement = Element;
            SubParams.ForEach(sp => sp.SetCurrentElementAsOriginalElement());
        }

        public virtual void Reset()
        {
            Deserialize(OriginalElement, false);
            SubParams.ForEach(sp => sp.Reset());
        }

#if CLIENT
        public SerializableEntityEditor SerializableEntityEditor { get; protected set; }
        public virtual void AddToEditor(ParamsEditor editor)
        {
            SerializableEntityEditor = new SerializableEntityEditor(editor.EditorBox.Content.RectTransform, this, inGame: false, showName: true);
            if (this is SpriteParams spriteParams && spriteParams.Deformation != null)
            {
                foreach (var deformation in spriteParams.Deformation.Deformations.Keys)
                {
                    new SerializableEntityEditor(editor.EditorBox.Content.RectTransform, deformation, inGame: false, showName: true);
                }
            }
            foreach (var subParam in SubParams)
            {
                subParam.AddToEditor(editor);
            }
        }
#endif
    }
}
