﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace KailashEngine.Animation
{
    class SkeletonAnimator
    {

        protected struct KeyFrame
        {
            // The time this key frame triggers
            public float time;

            // Frame data
            public Matrix4 data;


            public KeyFrame(float time, Matrix4 data)
            {
                this.time = time;
                this.data = data;
            }
        };


        //------------------------------------------------------
        // Data
        //------------------------------------------------------

        private string _id;
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        private float _global_last_frame_time;



        //------------------------------------------------------
        // Key Frame dictionaries
        //------------------------------------------------------


        private Dictionary<string, Dictionary<float, KeyFrame>> _key_frames_skeleton;



        //------------------------------------------------------
        // Constructor
        //------------------------------------------------------

        public SkeletonAnimator(string id)
        {
            _id = id;
            _global_last_frame_time = 0.0f;

            _key_frames_skeleton = new Dictionary<string, Dictionary<float, KeyFrame>>();
        }




        //------------------------------------------------------
        // Build Key Frame Dictionaries
        //------------------------------------------------------


        // Add key frames into skeleton dictionary
        public void addKeyFrame(string bone_name, float time, Matrix4 data)
        {
            Dictionary<float, KeyFrame> temp_dictionary = null;

            if (_key_frames_skeleton.TryGetValue(bone_name, out temp_dictionary))
            {
                temp_dictionary.Add(time, new KeyFrame(time, data));
            }
            else
            {
                temp_dictionary = new Dictionary<float, KeyFrame>();
                temp_dictionary.Add(time, new KeyFrame(time, data));
                _key_frames_skeleton.Add(bone_name, temp_dictionary);
            }
        }



        //------------------------------------------------------
        // Get Animation Data
        //------------------------------------------------------

        // Get the skelton's bone matrices at the specified time
        public Dictionary<string, Matrix4> getKeyFrame(float time)
        {
            Dictionary<string, Matrix4> temp_bone_matrices = new Dictionary<string, Matrix4>();

            foreach (KeyValuePair<string, Dictionary<float, KeyFrame>> keypair in _key_frames_skeleton)
            {
                string temp_bone_name = keypair.Key;

                List<float> key_frame_times = keypair.Value.Keys.ToList();


                // Get prevous and next frame with interpolation between them
                Vector3 PrevNextInterp = AnimationHelper.getNearestFrame(key_frame_times.ToArray(), time);

                // Default to the most recent frame's data
                Matrix4 output = keypair.Value[PrevNextInterp.X].data;

                if (PrevNextInterp.Z != -1)
                {
                    KeyFrame previous_frame = keypair.Value[PrevNextInterp.X];
                    KeyFrame next_frame = keypair.Value[PrevNextInterp.Y];
                    float interpolation = PrevNextInterp.Z;

                    output = EngineHelper.lerp(previous_frame.data, next_frame.data, interpolation);
                }

                temp_bone_matrices.Add(temp_bone_name, output);
            }

            return temp_bone_matrices;
        }


    }
}
