import apollo from "../apollo";

import QUERY_RESOURCE_APPROVAL from "../graphql/Approval/Get.gql";
import MUTATION_APPROVE from "../graphql/Approval/Approve.gql";

export const getResourceApprovals = async input => {
  return await apollo.mutate({
    mutation: QUERY_RESOURCE_APPROVAL,
    variables: {
      input
    }
  });
};

export const approveResources = async input => {
  return await apollo.mutate({
    mutation: MUTATION_APPROVE,
    variables: {
      input
    }
  });
};
